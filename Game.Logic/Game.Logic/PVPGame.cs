// Type: Game.Logic.PVPGame
// Assembly: Game.Logic, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9A18B27C-A4A9-4461-81EF-9F668B32B435
// Assembly location: C:\Users\Jhon\Desktop\coisas drumond\Emuladores\Fight\Game.Logic.dll

using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Game.Logic
{
    public class PVPGame : BaseGame
    {
        private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<Player> m_redTeam;
        private float m_redAvgLevel;
        private List<Player> m_blueTeam;
        private float m_blueAvgLevel;
        private int BeginPlayerCount;
        private string teamAStr;
        private string teamBStr;
        private DateTime beginTime;

        public Player CurrentPlayer
        {
            get
            {
                return this.m_currentLiving as Player;
            }
        }

        static PVPGame()
        {
        }

        public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType)
            : base(id, roomId, map, roomType, gameType, timeType)
        {
            this.m_redTeam = new List<Player>();
            this.m_blueTeam = new List<Player>();
            StringBuilder stringBuilder1 = new StringBuilder();
            this.m_redAvgLevel = 0.0f;
            foreach (IGamePlayer gp in red)
            {
                IGamePlayer player = gp;
                PVPGame pvpGame1 = this;
                int num1;
                int num2 = (num1 = pvpGame1.PhysicalId) + 1;
                pvpGame1.PhysicalId = num2;
                int id1 = num1;
                PVPGame pvpGame2 = this;
                int team = 1;
                int hp = gp.PlayerCharacter.hp;
                Player fp = new Player(player, id1, (BaseGame)pvpGame2, team, hp);
                stringBuilder1.Append(gp.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = this.m_random.Next(0, 1) == 0 ? 1 : -1;
                this.AddPlayer(gp, fp);
                this.m_redTeam.Add(fp);
                this.m_redAvgLevel += (float)gp.PlayerCharacter.Grade;
            }
            this.m_redAvgLevel /= (float)this.m_redTeam.Count;
            this.teamAStr = ((object)stringBuilder1).ToString();
            StringBuilder stringBuilder2 = new StringBuilder();
            this.m_blueAvgLevel = 0.0f;
            foreach (IGamePlayer gp in blue)
            {
                IGamePlayer player = gp;
                PVPGame pvpGame1 = this;
                int num1;
                int num2 = (num1 = pvpGame1.PhysicalId) + 1;
                pvpGame1.PhysicalId = num2;
                int id1 = num1;
                PVPGame pvpGame2 = this;
                int team = 2;
                int hp = gp.PlayerCharacter.hp;
                Player fp = new Player(player, id1, (BaseGame)pvpGame2, team, hp);
                stringBuilder2.Append(gp.PlayerCharacter.ID).Append(",");
                fp.Reset();
                fp.Direction = this.m_random.Next(0, 1) == 0 ? 1 : -1;
                this.AddPlayer(gp, fp);
                this.m_blueTeam.Add(fp);
                this.m_blueAvgLevel += (float)gp.PlayerCharacter.Grade;
            }
            this.m_blueAvgLevel /= (float)blue.Count;
            this.teamBStr = ((object)stringBuilder2).ToString();
            this.BeginPlayerCount = this.m_redTeam.Count + this.m_blueTeam.Count;
            this.beginTime = DateTime.Now;
        }

        public void Prepare()
        {
            if (this.GameState != eGameState.Inited)
                return;
            this.SendCreateGame();
            this.m_gameState = eGameState.Prepared;
            this.CheckState(0);
        }

        public void StartLoading()
        {
            if (this.GameState != eGameState.Prepared)
                return;
            this.ClearWaitTimer();
            this.SendStartLoading(60);
            this.VaneLoading();
            this.AddAction((IAction)new WaitPlayerLoadingAction((BaseGame)this, 61000));
            this.m_gameState = eGameState.Loading;
        }

        public void StartGame()
        {
            if (this.GameState != eGameState.Loading)
                return;
            this.m_gameState = eGameState.Playing;
            this.ClearWaitTimer();
            this.SendSyncLifeTime();
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            MapPoint mapRandomPos = MapMgr.GetMapRandomPos(this.m_map.Info.ID);
            GSPacketIn pkg = new GSPacketIn((short)91);
            pkg.WriteByte((byte)99);
            pkg.WriteInt(allFightPlayers.Count);
            foreach (Player player in allFightPlayers)
            {
                player.Reset();
                Point playerPoint = this.GetPlayerPoint(mapRandomPos, player.Team);
                ((Physics)player).SetXY(playerPoint);
                this.m_map.AddPhysical((Physics)player);
                ((Physics)player).StartMoving();
                player.StartGame();
                pkg.WriteInt(player.Id);
                pkg.WriteInt(player.X);
                pkg.WriteInt(player.Y);
                pkg.WriteInt(player.Direction);
                pkg.WriteInt(player.Blood);
                pkg.WriteInt(player.MaxBlood);
                pkg.WriteInt(player.Team);
                pkg.WriteInt(player.Weapon.RefineryLevel);
                pkg.WriteInt(player.deputyWeaponCount);
                pkg.WriteInt(5);
                pkg.WriteInt(player.Dander);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteBoolean(player.IsFrost);
                pkg.WriteBoolean(player.IsHide);
                pkg.WriteBoolean(player.IsNoHole);
                pkg.WriteInt(0);
            }
            pkg.WriteInt(0);
            pkg.WriteInt(0);
            pkg.WriteDateTime(DateTime.Now);
            this.SendToAll(pkg);
            this.VaneLoading();
            this.WaitTime(allFightPlayers.Count * 1000);
            this.OnGameStarted();
        }

        public void NextTurn()
        {
            if (this.GameState != eGameState.Playing)
                return;
            this.ClearWaitTimer();
            this.ClearDiedPhysicals();
            this.CheckBox();
            PVPGame pvpGame = this;
            int num = pvpGame.m_turnIndex + 1;
            pvpGame.m_turnIndex = num;
            List<Box> box = this.CreateBox();
            foreach (Physics physics in this.m_map.GetAllPhysicalSafe())
                physics.PrepareNewTurn();
            this.m_currentLiving = this.FindNextTurnedLiving();
            if (this.m_currentLiving.vaneOpen)
                this.UpdateWind(this.GetNextWind(), false);
            this.MinusDelays(this.m_currentLiving.Delay);
            this.m_currentLiving.PrepareSelfTurn();
            if (!this.CurrentLiving.IsFrost && this.m_currentLiving.IsLiving)
            {
                this.m_currentLiving.StartAttacking();
                this.SendGameNextTurn((Living)this.m_currentLiving, (BaseGame)this, box);
                if (this.m_currentLiving.IsAttacking)
                    this.AddAction((IAction)new WaitLivingAttackingAction(this.m_currentLiving, this.m_turnIndex, (this.m_timeType + 20) * 1000));
            }
            this.OnBeginNewTurn();
        }

        public override bool TakeCard(Player player)
        {
            int index1 = 0;
            for (int index2 = 0; index2 < this.Cards.Length; ++index2)
            {
                if (this.Cards[index2] == 0)
                {
                    index1 = index2;
                    break;
                }
            }
            return this.TakeCard(player, index1);
        }

        public override bool TakeCard(Player player, int index)
        {
            if (player.CanTakeOut == 0 || !player.IsActive || (index < 0 || index > this.Cards.Length) || player.FinishTakeCard || this.Cards[index] > 0)
                return false;
            --player.CanTakeOut;
            int gold = 0;
            int money = 0;
            int giftToken = 0;
            int medal = 0;
            int templateID = 0;
            int count = 0;
            List<SqlDataProvider.Data.ItemInfo> info = (List<SqlDataProvider.Data.ItemInfo>)null;
            if (DropInventory.CardDrop(this.RoomType, ref info) && info != null)
            {
                foreach (SqlDataProvider.Data.ItemInfo itemInfo in info)
                {
                    templateID = itemInfo.TemplateID;
                    count = itemInfo.Count;
                    SqlDataProvider.Data.ItemInfo.FindSpecialItemInfo(itemInfo, ref gold, ref money, ref giftToken, ref medal);
                    if (templateID > 0)
                        player.PlayerDetail.AddTemplate(itemInfo, eBageType.TempBag, itemInfo.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.NoneTypeView);
                }
            }
            player.FinishTakeCard = true;
            this.Cards[index] = 1;
            player.PlayerDetail.AddGold(gold);
            player.PlayerDetail.AddMoney(money);
            player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
            player.PlayerDetail.AddGiftToken(giftToken);
            this.SendGamePlayerTakeCard(player, index, templateID, count, false);
            return true;
        }

        private int CalculateExperience(Player player, int winTeam, ref int reward)
        {
            if (this.m_roomType != eRoomType.Match)
                return 0;
            float num1 = player.Team == 1 ? this.m_blueAvgLevel : this.m_redAvgLevel;
            float num2 = player.Team == 1 ? (float)this.m_blueTeam.Count : (float)this.m_redTeam.Count;
            double num3 = (double)Math.Abs(num1 - (float)player.PlayerDetail.PlayerCharacter.Grade);
            if (player.TotalHurt == 0)
            {
                if ((double)num1 - (double)this.m_blueAvgLevel < 5.0 && (double)num1 - (double)this.m_redAvgLevel < 5.0 || this.TotalHurt <= 0)
                    return 1;
                this.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), (string)null, 2);
                reward = 200;
                return 201;
            }
            else
            {
                float num4 = player.Team == winTeam ? 2f : 0.0f;
                player.TotalShootCount = player.TotalShootCount == 0 ? 1 : player.TotalShootCount;
                if (player.TotalShootCount < player.TotalHitTargetCount)
                    player.TotalShootCount = player.TotalHitTargetCount;
                int num5 = player.Team == 1 ? (int)((double)this.m_blueTeam.Count * (double)this.m_blueAvgLevel * 300.0) : (int)((double)this.m_redAvgLevel * (double)this.m_redTeam.Count * 300.0);
                int num6 = player.TotalHurt > num5 ? num5 : player.TotalHurt;
                int gp = (int)Math.Ceiling(((double)num4 + (double)num6 * 0.009 + (double)player.TotalKill * 0.5 + (double)(player.TotalHitTargetCount / player.TotalShootCount * 2)) * (double)num1 * (0.9 + ((double)num2 - 1.0) * 0.3));
                if (((double)num1 - (double)this.m_blueAvgLevel >= 5.0 || (double)num1 - (double)this.m_redAvgLevel >= 5.0) && this.TotalHurt > 0)
                {
                    this.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), (string)null, 2);
                    reward = 200;
                    gp += 200;
                }
                int num7 = this.GainCoupleGP(player, gp);
                if (num7 > 100000)
                {
                    PVPGame.log.Error((object)string.Format("pvpgame ====== player.nickname : {0}, add gp : {1} ======== gp > 10000", (object)player.PlayerDetail.PlayerCharacter.NickName, (object)num7));
                    PVPGame.log.Error((object)string.Format("pvpgame ====== player.nickname : {0}, parameters winPlus: {1}, totalHurt : {2}, totalKill : {3}, totalHitTargetCount : {4}, totalShootCount : {5}, againstTeamLevel : {6}, againstTeamCount : {7}", (object)player.PlayerDetail.PlayerCharacter.NickName, (object)num4, (object)player.TotalHurt, (object)player.TotalKill, (object)player.TotalHitTargetCount, (object)player.TotalShootCount, (object)num1, (object)num2));
                }
                if (num7 >= 1)
                    return num7;
                else
                    return 1;
            }
        }

        public int GainCoupleGP(Player player, int gp)
        {
            foreach (Player player1 in this.GetSameTeamPlayer(player))
            {
                if (player1.PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.SpouseID)
                    return (int)((double)gp * 1.2);
            }
            return gp;
        }

        public Player[] GetSameTeamPlayer(Player player)
        {
            List<Player> list = new List<Player>();
            foreach (Player player1 in this.GetAllFightPlayers())
            {
                if (player1 != player && player1.Team == player.Team)
                    list.Add(player1);
            }
            return list.ToArray();
        }

        public void GameOver()
        {
            if (this.GameState != eGameState.Playing)
                return;
            this.m_gameState = eGameState.GameOver;
            this.ClearWaitTimer();
            this.CurrentTurnTotalDamage = 0;
            List<Player> allFightPlayers = this.GetAllFightPlayers();
            int num1 = -1;
            foreach (Player player in allFightPlayers)
            {
                if (player.IsLiving)
                {
                    num1 = player.Team;
                    break;
                }
            }
            if (num1 == -1 && this.CurrentPlayer != null)
                num1 = this.CurrentPlayer.Team;
            int val1 = this.CalculateGuildMatchResult(allFightPlayers, num1);
            if (this.RoomType == eRoomType.Match && this.GameType == eGameType.Guild)
            {
                int num2 = 10;
                int num3 = -10;
                int num4 = num2 + allFightPlayers.Count / 2;
                int num5 = num3 + (int)Math.Round((double)(allFightPlayers.Count / 2) * 0.5);
            }
            int num6 = 0;
            int num7 = 0;
            foreach (Player player in allFightPlayers)
            {
                if (player.TotalHurt > 0)
                {
                    if (player.Team == 1)
                        num7 = 1;
                    else
                        num6 = 1;
                }
            }
            int val2 = 0;
            GSPacketIn pkg = new GSPacketIn((short)91);
            pkg.WriteByte((byte)100);
            pkg.WriteInt(val2);
            pkg.WriteInt(this.PlayerCount);
            foreach (Player player in allFightPlayers)
            {
                float num2 = player.Team == 1 ? this.m_blueAvgLevel : this.m_redAvgLevel;
                if (player.Team != 1)
                {
                    int count1 = this.m_redTeam.Count;
                }
                else
                {
                    int count2 = this.m_blueTeam.Count;
                }
                float num3 = Math.Abs(num2 - (float)player.PlayerDetail.PlayerCharacter.Grade);
                int team = player.Team;
                int num4 = 0;
                int reward = 0;
                if (player.TotalShootCount != 0)
                {
                    int num5 = player.TotalShootCount;
                }
                if (this.m_roomType == eRoomType.Match || (double)num3 < 5.0)
                    num4 = this.CalculateExperience(player, num1, ref reward);
                int num8 = (num4 == 0 ? 1 : num4) / 10;
                player.CanTakeOut = player.Team == 1 ? num7 : num6;
                val1 += player.GainOffer;
                float num9 = player.Team == num1 ? 2f : 0.0f;
                if (this.RoomType != eRoomType.Freedom)
                {
                    Random random = new Random();
                    int num10 = (double)num9 != 2.0 ? 50 : 300;
                    string str = DateTime.Now.ToString("HH");
                    int num11 = 0;
                    if (str == "5" || str == "9" || str == "13")
                        num11 = 90000;
                    else if (str == "21" || str == "23")
                        num11 = 90000;
                    else if (str == "0" || str == "24")
                        num11 = num8;
                    string msg = "Você ganhou " + (object)num8 + " experiência e " + (string)(object)num10 + " cupons.";
                    if (num11 > 0)
                    {
                        msg = string.Concat(new object[4]
            {
              (object) msg,
              (object) " é um prazer te-lo aqui com nosco! ",
              (object) num11,
              (object) " Desejo-lhe um bom jogo."
            });
                        num8 += num11;
                    }
                    player.PlayerDetail.AddMoney(num10);
                    player.PlayerDetail.SendMessage(msg);
                }
                player.GainGP = player.PlayerDetail.AddGP(num8);
                player.GainOffer = player.PlayerDetail.AddOffer(val1);
                pkg.WriteInt(player.Id);
                pkg.WriteBoolean(player.Team == num1);
                pkg.WriteInt(player.Grade);
                pkg.WriteInt(player.PlayerDetail.PlayerCharacter.GP);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(num8);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(player.GainGP);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(0);
                pkg.WriteInt(player.GainOffer);
                pkg.WriteInt(0);
                pkg.WriteInt(player.CanTakeOut);
            }
            pkg.WriteInt(val1);
            pkg.WriteInt(0);
            pkg.WriteInt(0);
            this.SendToAll(pkg);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Player player in allFightPlayers)
                player.PlayerDetail.OnGameOver((AbstractGame)this, player.Team == num1, player.GainGP);
            this.OnGameOverLog(this.RoomId, this.RoomType, this.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayerCount, this.Map.Info.ID, this.teamAStr, this.teamBStr, "", num1, this.BossWarField);
            this.WaitTime(15000);
            this.OnGameOverred();
        }

        public override void Stop()
        {
            if (this.GameState != eGameState.GameOver)
                return;
            this.m_gameState = eGameState.Stopped;
            foreach (Player player in this.GetAllFightPlayers())
            {
                if (player.IsActive && !player.FinishTakeCard && player.CanTakeOut > 0)
                    this.TakeCard(player);
            }
            lock (this.m_players)
                this.m_players.Clear();
            base.Stop();
        }

        private int CalculateGuildMatchResult(List<Player> players, int winTeam)
        {
            if (this.RoomType == eRoomType.Match)
            {
                StringBuilder stringBuilder1 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                StringBuilder stringBuilder2 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
                IGamePlayer gamePlayer1 = (IGamePlayer)null;
                IGamePlayer gamePlayer2 = (IGamePlayer)null;
                int num = 0;
                foreach (Player player in players)
                {
                    if (player.Team == winTeam)
                    {
                        stringBuilder1.Append(string.Format("[{0}]", (object)player.PlayerDetail.PlayerCharacter.NickName));
                        gamePlayer1 = player.PlayerDetail;
                    }
                    else
                    {
                        stringBuilder2.Append(string.Format("{0}", (object)player.PlayerDetail.PlayerCharacter.NickName));
                        gamePlayer2 = player.PlayerDetail;
                        ++num;
                    }
                }
                if (gamePlayer2 != null)
                {
                    stringBuilder1.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1", new object[0]) + gamePlayer2.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2", new object[0]));
                    stringBuilder2.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3", new object[0]) + gamePlayer1.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4", new object[0]));
                    int riches = 0;
                    if (this.GameType == eGameType.Guild)
                        riches = num + this.TotalHurt / 2000;
                    gamePlayer1.ConsortiaFight(gamePlayer1.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, this.Players, this.RoomType, this.GameType, this.TotalHurt, players.Count);
                    if (gamePlayer1.ServerID != gamePlayer2.ServerID)
                        gamePlayer2.ConsortiaFight(gamePlayer1.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, this.Players, this.RoomType, this.GameType, this.TotalHurt, players.Count);
                    if (this.GameType == eGameType.Guild)
                        gamePlayer1.SendConsortiaFight(gamePlayer1.PlayerCharacter.ConsortiaID, riches, ((object)stringBuilder1).ToString());
                    return riches;
                }
            }
            return 0;
        }

        public bool CanGameOver()
        {
            bool flag1 = true;
            bool flag2 = true;
            foreach (Physics physics in this.m_redTeam)
            {
                if (physics.IsLiving)
                {
                    flag1 = false;
                    break;
                }
            }
            foreach (Physics physics in this.m_blueTeam)
            {
                if (physics.IsLiving)
                {
                    flag2 = false;
                    break;
                }
            }
            return flag1 || flag2;
        }

        public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
        {
            Player player = base.RemovePlayer(gp, IsKick);
            if (player != null && player.IsLiving && this.GameState != eGameState.Loading)
            {
                gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
                string msg = (string)null;
                string msg1 = (string)null;
                if (this.RoomType == eRoomType.Match)
                {
                    if (this.GameType == eGameType.Guild)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", (object)(gp.PlayerCharacter.Grade * 12), (object)15);
                        gp.RemoveOffer(15);
                        msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", (object)gp.PlayerCharacter.NickName, (object)(gp.PlayerCharacter.Grade * 12), (object)15);
                    }
                    else if (this.GameType == eGameType.Free)
                    {
                        msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", (object)(gp.PlayerCharacter.Grade * 12), (object)5);
                        gp.RemoveOffer(5);
                        msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", (object)gp.PlayerCharacter.NickName, (object)(gp.PlayerCharacter.Grade * 12), (object)5);
                    }
                }
                else
                {
                    msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[1]
          {
            (object) (gp.PlayerCharacter.Grade * 12)
          });
                    msg1 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", (object)gp.PlayerCharacter.NickName, (object)(gp.PlayerCharacter.Grade * 12));
                }
                this.SendMessage(gp, msg, msg1, 3);
                if (this.GetSameTeam())
                {
                    this.CurrentLiving.StopAttacking();
                    this.CheckState(0);
                }
            }
            return player;
        }

        public override void CheckState(int delay)
        {
            this.AddAction((IAction)new CheckPVPGameStateAction(delay));
        }
    }
}
