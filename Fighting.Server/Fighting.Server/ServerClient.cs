using Bussiness;
using Bussiness.Managers;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Fighting.Server
{
    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();
        private RSACryptoServiceProvider m_rsa;
        private FightServer m_svr;

        static ServerClient()
        {
        }

        public ServerClient(FightServer svr)
            : base(new byte[8192], new byte[8192])
        {
            this.m_svr = svr;
        }

        protected override void OnConnect()
        {
            base.OnConnect();
            this.m_rsa = new RSACryptoServiceProvider();
            RSAParameters rsaParameters = this.m_rsa.ExportParameters(false);
            this.SendRSAKey(rsaParameters.Modulus, rsaParameters.Exponent);
        }

        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this.m_rsa = (RSACryptoServiceProvider)null;
        }

        public override void OnRecvPacket(GSPacketIn pkg)
        {
            switch (pkg.Code)
            {
                case (short)64:
                    this.HandleGameRoomCreate(pkg);
                    break;
                case (short)65:
                    this.HandleGameRoomCancel(pkg);
                    break;
                case (short)69:
                    this.HandleConsortiaAlly(pkg);
                    break;
                case (short)83:
                    this.HandlePlayerExit(pkg);
                    break;
                case (short)1:
                    this.HandleLogin(pkg);
                    break;
                case (short)2:
                    this.HanleSendToGame(pkg);
                    break;
                case (short)3:
                    this.HandleSysNotice(pkg);
                    break;
                case (short)19:
                    this.HandlePlayerMessage(pkg);
                    break;
                case (short)36:
                    this.HandlePlayerUsingProp(pkg);
                    break;
            }
        }

        private void HandlePlayerUsingProp(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            game.Resume();
            if (pkg.ReadBoolean())
            {
                Player player = game.FindPlayer(pkg.Parameter1);
                ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(pkg.Parameter2);
                if (player != null && itemTemplate != null)
                    player.UseItem(itemTemplate);
            }
        }

        private void HandlePlayerExit(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            Player player = game.FindPlayer(pkg.Parameter1);
            if (player != null)
            {
                GSPacketIn pkg1 = new GSPacketIn((short)83, player.PlayerDetail.PlayerCharacter.ID);
                game.SendToAll(pkg1);
                game.RemovePlayer(player.PlayerDetail, false);
                ProxyRoom roomUnsafe1 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Red.RoomId);
                if (roomUnsafe1 != null && !roomUnsafe1.RemovePlayer(player.PlayerDetail))
                {
                    ProxyRoom roomUnsafe2 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Blue.RoomId);
                    if (roomUnsafe2 != null)
                        roomUnsafe2.RemovePlayer(player.PlayerDetail);
                }
            }
        }

        public void HandleConsortiaAlly(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            game.ConsortiaAlly = pkg.ReadInt();
            game.RichesRate = pkg.ReadInt();
        }

        private void HandleSysNotice(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            Player player = game.FindPlayer(pkg.Parameter1);
            GSPacketIn pkg1 = new GSPacketIn((short)3);
            pkg1.WriteInt(3);
            pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", (object)(player.PlayerDetail.PlayerCharacter.Grade * 12), (object)15));
            player.PlayerDetail.SendTCP(pkg1);
            pkg1.ClearContext();
            pkg1.WriteInt(3);
            pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", (object)player.PlayerDetail.PlayerCharacter.NickName, (object)(player.PlayerDetail.PlayerCharacter.Grade * 12), (object)15));
            game.SendToAll(pkg1, player.PlayerDetail);
        }

        private void HandlePlayerMessage(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            Player player = game.FindPlayer(pkg.ReadInt());
            bool flag = pkg.ReadBoolean();
            string str = pkg.ReadString();
            if (player != null)
            {
                GSPacketIn pkg1 = new GSPacketIn((short)19);
                pkg1.ClientID = player.PlayerDetail.PlayerCharacter.ID;
                pkg1.WriteByte((byte)5);
                pkg1.WriteBoolean(false);
                pkg1.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
                pkg1.WriteString(str);
                if (flag)
                    game.SendToTeam(pkg, player.Team);
                else
                    game.SendToAll(pkg1);
            }
        }

        public void HandleLogin(GSPacketIn pkg)
        {
            string[] strArray = Encoding.UTF8.GetString(this.m_rsa.Decrypt(pkg.ReadBytes(), false)).Split(new char[1]
      {
        ','
      });
            if (strArray.Length == 2)
            {
                this.m_rsa = (RSACryptoServiceProvider)null;
                int.Parse(strArray[0]);
                this.Strict = false;
            }
            else
            {
                ServerClient.log.ErrorFormat("Error Login Packet from {0}", (object)this.TcpEndpoint);
                this.Disconnect();
            }
        }

        public void HandleGameRoomCreate(GSPacketIn pkg)
        {
            int num1 = pkg.ReadInt();
            int num2 = pkg.ReadInt();
            int num3 = pkg.ReadInt();
            int length = pkg.ReadInt();
            int num4 = 0;
            IGamePlayer[] players = new IGamePlayer[length];
            for (int index1 = 0; index1 < length; ++index1)
            {
                PlayerInfo character = new PlayerInfo();
                character.ID = pkg.ReadInt();
                character.NickName = pkg.ReadString();
                character.Sex = pkg.ReadBoolean();
                character.typeVIP = pkg.ReadByte();
                character.VIPLevel = pkg.ReadInt();
                character.Hide = pkg.ReadInt();
                character.Style = pkg.ReadString();
                character.Colors = pkg.ReadString();
                character.Skin = pkg.ReadString();
                character.Offer = pkg.ReadInt();
                character.GP = pkg.ReadInt();
                character.Grade = pkg.ReadInt();
                character.Repute = pkg.ReadInt();
                character.ConsortiaID = pkg.ReadInt();
                character.ConsortiaName = pkg.ReadString();
                character.ConsortiaLevel = pkg.ReadInt();
                character.ConsortiaRepute = pkg.ReadInt();
                character.badgeID = pkg.ReadInt();
                character.weaklessGuildProgress = Base64.decodeToByteArray(pkg.ReadString());
                character.Honor = pkg.ReadString();
                character.Attack = pkg.ReadInt();
                character.Defence = pkg.ReadInt();
                character.Agility = pkg.ReadInt();
                character.Luck = pkg.ReadInt();
                character.hp = pkg.ReadInt();
                character.FightPower = pkg.ReadInt();
                character.IsMarried = pkg.ReadBoolean();
                if (character.IsMarried)
                {
                    character.SpouseID = pkg.ReadInt();
                    character.SpouseName = pkg.ReadString();
                }
                ProxyPlayerInfo proxyPlayer = new ProxyPlayerInfo();
                proxyPlayer.BaseAttack = pkg.ReadDouble();
                proxyPlayer.BaseDefence = pkg.ReadDouble();
                proxyPlayer.BaseAgility = pkg.ReadDouble();
                proxyPlayer.BaseBlood = pkg.ReadDouble();
                proxyPlayer.TemplateId = pkg.ReadInt();
                proxyPlayer.CanUserProp = pkg.ReadBoolean();
                proxyPlayer.SecondWeapon = pkg.ReadInt();
                proxyPlayer.StrengthLevel = pkg.ReadInt();
                proxyPlayer.Healstone = pkg.ReadInt();
                proxyPlayer.HealstoneCount = pkg.ReadInt();
                proxyPlayer.GPAddPlus = pkg.ReadDouble();
                proxyPlayer.OfferAddPlus = pkg.ReadDouble();
                proxyPlayer.AntiAddictionRate = pkg.ReadDouble();
                proxyPlayer.ServerId = pkg.ReadInt();
                UsersPetinfo pet = new UsersPetinfo();
                if (pkg.ReadInt() == 1)
                {
                    pet.Place = pkg.ReadInt();
                    pet.TemplateID = pkg.ReadInt();
                    pet.ID = pkg.ReadInt();
                    pet.Name = pkg.ReadString();
                    pet.UserID = pkg.ReadInt();
                    pet.Level = pkg.ReadInt();
                    pet.Skill = pkg.ReadString();
                    pet.SkillEquip = pkg.ReadString();
                }
                else
                    pet = (UsersPetinfo)null;
                List<BufferInfo> infos = new List<BufferInfo>();
                int num5 = pkg.ReadInt();
                for (int index2 = 0; index2 < num5; ++index2)
                {
                    BufferInfo bufferInfo = new BufferInfo();
                    bufferInfo.Type = pkg.ReadInt();
                    bufferInfo.IsExist = pkg.ReadBoolean();
                    bufferInfo.BeginDate = pkg.ReadDateTime();
                    bufferInfo.ValidDate = pkg.ReadInt();
                    bufferInfo.Value = pkg.ReadInt();
                    bufferInfo.ValidCount = pkg.ReadInt();
                    if (character != null)
                        infos.Add(bufferInfo);
                }
                List<ItemInfo> euipEffects = new List<ItemInfo>();
                int num6 = pkg.ReadInt();
                for (int index2 = 0; index2 < num6; ++index2)
                {
                    int templateId = pkg.ReadInt();
                    int num7 = pkg.ReadInt();
                    ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 1);
                    fromTemplate.Hole1 = num7;
                    euipEffects.Add(fromTemplate);
                }
                players[index1] = (IGamePlayer)new ProxyPlayer(this, character, proxyPlayer, pet, infos, euipEffects);
                players[index1].CanUseProp = proxyPlayer.CanUserProp;
                num4 += character.Grade;
            }
            ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomId(), num1, players, this);
            room.GuildId = num3;
            room.GameType = (eGameType)num2;
            lock (this.m_rooms)
            {
                if (!this.m_rooms.ContainsKey(num1))
                    this.m_rooms.Add(num1, room);
                else
                    room = (ProxyRoom)null;
            }
            if (room != null)
                ProxyRoomMgr.AddRoom(room);
            else
                ServerClient.log.ErrorFormat("Room already exists:{0}", (object)num1);
        }

        public void HandleGameRoomCancel(GSPacketIn pkg)
        {
            ProxyRoom room = (ProxyRoom)null;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(pkg.Parameter1))
                    room = this.m_rooms[pkg.Parameter1];
            }
            if (room == null)
                return;
            ProxyRoomMgr.RemoveRoom(room);
        }

        public void HanleSendToGame(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game == null)
                return;
            GSPacketIn pkg1 = pkg.ReadPacket();
            game.ProcessData(pkg1);
        }

        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn((short)0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }

        public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
        {
            GSPacketIn pkg1 = new GSPacketIn((short)32, playerId);
            pkg1.WritePacket(pkg);
            this.SendTCP(pkg1);
        }

        public void SendRemoveRoom(int roomId)
        {
            this.SendTCP(new GSPacketIn((short)65, roomId));
        }

        public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
        {
            GSPacketIn pkg1 = new GSPacketIn((short)67, roomId);
            if (except != null)
            {
                pkg1.Parameter1 = except.PlayerCharacter.ID;
                pkg1.Parameter2 = except.GamePlayerId;
            }
            else
            {
                pkg1.Parameter1 = 0;
                pkg1.Parameter2 = 0;
            }
            pkg1.WritePacket(pkg);
            this.SendTCP(pkg1);
        }

        public void SendStartGame(int roomId, AbstractGame game)
        {
            GSPacketIn pkg = new GSPacketIn((short)66);
            pkg.Parameter1 = roomId;
            pkg.Parameter2 = game.Id;
            pkg.WriteInt((int)game.RoomType);
            pkg.WriteInt((int)game.GameType);
            pkg.WriteInt(game.TimeType);
            this.SendTCP(pkg);
        }

        public void SendStopGame(int roomId, int gameId)
        {
            this.SendTCP(new GSPacketIn((short)68)
            {
                Parameter1 = roomId,
                Parameter2 = gameId
            });
        }

        public void SendGamePlayerId(IGamePlayer player)
        {
            this.SendTCP(new GSPacketIn((short)33)
            {
                Parameter1 = player.PlayerCharacter.ID,
                Parameter2 = player.GamePlayerId
            });
        }

        public void SendDisconnectPlayer(int playerId)
        {
            this.SendTCP(new GSPacketIn((short)34, playerId));
        }

        public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
        {
            GSPacketIn pkg = new GSPacketIn((short)35, playerId);
            pkg.Parameter1 = gameId;
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(gainXp);
            this.SendTCP(pkg);
        }

        public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
        {
            GSPacketIn pkg = new GSPacketIn((short)36, playerId);
            pkg.Parameter1 = bag;
            pkg.Parameter2 = place;
            pkg.WriteInt(templateId);
            pkg.WriteBoolean(isLiving);
            this.SendTCP(pkg);
        }

        public void SendPlayerAddGold(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)38, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerAddMoney(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)70, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerAddGiftToken(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)71, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerRemoveHealstone(int playerId)
        {
            this.SendTCP(new GSPacketIn((short)73, playerId));
        }

        public void SendPlayerAddMedal(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)72, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerAddGP(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)39, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerRemoveGP(int playerId, int value)
        {
            this.SendTCP(new GSPacketIn((short)49, playerId)
            {
                Parameter1 = value
            });
        }

        public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
        {
            GSPacketIn pkg = new GSPacketIn((short)40, playerId);
            pkg.WriteInt(type);
            pkg.WriteBoolean(isLiving);
            pkg.WriteInt(demage);
            this.SendTCP(pkg);
        }

        public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
        {
            GSPacketIn pkg = new GSPacketIn((short)41, playerId);
            pkg.WriteBoolean(isWin);
            pkg.WriteInt(MissionID);
            pkg.WriteInt(turnNum);
            this.SendTCP(pkg);
        }

        public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
        {
            GSPacketIn pkg = new GSPacketIn((short)42, playerId);
            pkg.WriteInt(consortiaWin);
            pkg.WriteInt(consortiaLose);
            pkg.WriteInt(players.Count);
            for (int index = 0; index < players.Count; ++index)
                pkg.WriteInt(players[index].PlayerDetail.PlayerCharacter.ID);
            pkg.WriteByte((byte)roomType);
            pkg.WriteByte((byte)gameClass);
            pkg.WriteInt(totalKillHealth);
            this.SendTCP(pkg);
        }

        public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
        {
            GSPacketIn pkg = new GSPacketIn((short)43, playerId);
            pkg.WriteInt(consortiaID);
            pkg.WriteInt(riches);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveGold(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((short)44, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveMoney(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((short)45, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerRemoveOffer(int playerId, int value)
        {
            GSPacketIn pkg = new GSPacketIn((short)50, playerId);
            pkg.WriteInt(value);
            this.SendTCP(pkg);
        }

        public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
        {
            if (cloneItem == null)
                return;
            GSPacketIn pkg = new GSPacketIn((short)48, playerId);
            pkg.WriteInt(cloneItem.TemplateID);
            pkg.WriteByte((byte)bagType);
            pkg.WriteInt(count);
            pkg.WriteInt(cloneItem.ValidDate);
            pkg.WriteBoolean(cloneItem.IsBinds);
            pkg.WriteBoolean(cloneItem.IsUsed);
            this.SendTCP(pkg);
        }

        public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
        {
            GSPacketIn pkg = new GSPacketIn((short)69);
            pkg.WriteInt(Consortia1);
            pkg.WriteInt(Consortia2);
            pkg.WriteInt(GameId);
            this.SendTCP(pkg);
        }

        public override string ToString()
        {
            return string.Format("Server Client: {0} IsConnected:{1}  RoomCount:{2}", (object)0, (object)(int)(this.IsConnected ? 1 : 0), (object)this.m_rooms.Count);
        }

        public void RemoveRoom(int orientId, ProxyRoom room)
        {
            bool flag = false;
            lock (this.m_rooms)
            {
                if (this.m_rooms.ContainsKey(orientId) && this.m_rooms[orientId] == room)
                    flag = this.m_rooms.Remove(orientId);
            }
            if (!flag)
                return;
            this.SendRemoveRoom(orientId);
        }
    }
}
