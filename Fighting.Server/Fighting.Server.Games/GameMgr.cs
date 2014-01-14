using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Fighting.Server.Games
{
    public class GameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly long THREAD_INTERVAL = 40L;
        private static readonly int CLEAR_GAME_INTERVAL = 60000;
        private static Dictionary<int, BaseGame> m_games;
        private static Thread m_thread;
        private static bool m_running;
        private static int m_serverId;
        private static int m_boxBroadcastLevel;
        private static int m_gameId;
        private static long m_clearGamesTimer;

        public static int BoxBroadcastLevel
        {
            get
            {
                return GameMgr.m_boxBroadcastLevel;
            }
        }

        static GameMgr()
        {
        }

        public static bool Setup(int serverId, int boxBroadcastLevel)
        {
            GameMgr.m_thread = new Thread(new ThreadStart(GameMgr.GameThread));
            GameMgr.m_games = new Dictionary<int, BaseGame>();
            GameMgr.m_serverId = serverId;
            GameMgr.m_boxBroadcastLevel = boxBroadcastLevel;
            GameMgr.m_gameId = 0;
            return true;
        }

        public static void Start()
        {
            if (GameMgr.m_running)
                return;
            GameMgr.m_running = true;
            GameMgr.m_thread.Start();
        }

        public static void Stop()
        {
            if (!GameMgr.m_running)
                return;
            GameMgr.m_running = false;
            GameMgr.m_thread.Join();
        }

        private static void GameThread()
        {
            long num = 0L;
            GameMgr.m_clearGamesTimer = TickHelper.GetTickCount();
            while (GameMgr.m_running)
            {
                long tickCount1 = TickHelper.GetTickCount();
                try
                {
                    GameMgr.UpdateGames(tickCount1);
                    GameMgr.ClearStoppedGames(tickCount1);
                }
                catch (Exception ex)
                {
                    GameMgr.log.Error((object)"Room Mgr Thread Error:", ex);
                }
                long tickCount2 = TickHelper.GetTickCount();
                num += GameMgr.THREAD_INTERVAL - (tickCount2 - tickCount1);
                if (num > 0L)
                {
                    Thread.Sleep((int)num);
                    num = 0L;
                }
                else if (num < -1000L)
                {
                    GameMgr.log.WarnFormat("Room Mgr is delay {0} ms!", (object)num);
                    num += 1000L;
                }
            }
        }

        private static void UpdateGames(long tick)
        {
            IList list = (IList)GameMgr.GetGames();
            if (list == null)
                return;
            foreach (BaseGame baseGame in (IEnumerable)list)
            {
                try
                {
                    baseGame.Update(tick);
                }
                catch (Exception ex)
                {
                    GameMgr.log.Error((object)"Game  updated error:", ex);
                }
            }
        }

        private static void ClearStoppedGames(long tick)
        {
            if (GameMgr.m_clearGamesTimer > tick)
                return;
            GameMgr.m_clearGamesTimer += (long)GameMgr.CLEAR_GAME_INTERVAL;
            ArrayList arrayList = new ArrayList();
            lock (GameMgr.m_games)
            {
                foreach (BaseGame item_0 in GameMgr.m_games.Values)
                {
                    if (item_0.GameState == eGameState.Stopped)
                        arrayList.Add((object)item_0);
                }
                foreach (BaseGame item_1 in arrayList)
                {
                    GameMgr.m_games.Remove(item_1.Id);
                    try
                    {
                        item_1.Dispose();
                    }
                    catch (Exception exception_0)
                    {
                        GameMgr.log.Error((object)"game dispose error:", exception_0);
                    }
                }
            }
        }

        public static List<BaseGame> GetGames()
        {
            List<BaseGame> list = new List<BaseGame>();
            lock (GameMgr.m_games)
                list.AddRange((IEnumerable<BaseGame>)GameMgr.m_games.Values);
            return list;
        }

        public static BaseGame FindGame(int id)
        {
            lock (GameMgr.m_games)
            {
                if (GameMgr.m_games.ContainsKey(id))
                    return GameMgr.m_games[id];
            }
            return (BaseGame)null;
        }

        public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            BaseGame baseGame;
            try
            {
                Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId));
                if (map != null)
                {
                    PVPGame pvpGame = new PVPGame(GameMgr.m_gameId++, 0, red, blue, map, roomType, gameType, timeType);
                    lock (GameMgr.m_games)
                        GameMgr.m_games.Add(pvpGame.Id, (BaseGame)pvpGame);
                    pvpGame.Prepare();
                    baseGame = (BaseGame)pvpGame;
                }
                else
                    baseGame = (BaseGame)null;
            }
            catch (Exception ex)
            {
                GameMgr.log.Error((object)"Create game error:", ex);
                baseGame = (BaseGame)null;
            }
            return baseGame;
        }

        public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
        {
            BattleGame battleGame;
            try
            {
                Map map = MapMgr.CloneMap(MapMgr.GetMapIndex(mapIndex, (byte)roomType, GameMgr.m_serverId));
                if (map != null)
                {
                    BattleGame game = new BattleGame(GameMgr.m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType);
                    lock (GameMgr.m_games)
                        GameMgr.m_games.Add(game.Id, (BaseGame)game);
                    game.Prepare();
                    GameMgr.SendStartMessage(game);
                    battleGame = game;
                }
                else
                    battleGame = (BattleGame)null;
            }
            catch (Exception ex)
            {
                GameMgr.log.Error((object)"Create battle game error:", ex);
                battleGame = (BattleGame)null;
            }
            return battleGame;
        }

        public static void SendStartMessage(BattleGame game)
        {
            GSPacketIn pkg1 = new GSPacketIn((short)3);
            pkg1.WriteInt(2);
            if (game.GameType == eGameType.Free)
            {
                foreach (Player player in game.GetAllFightPlayers())
                {
                    (player.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
                    GSPacketIn pkg2 = GameMgr.SendBufferList(player, (player.PlayerDetail as ProxyPlayer).Buffers);
                    game.SendToAll(pkg2);
                }
                pkg1.WriteString("Processo de busca sucedido! A sua equipe começará o combate!");
            }
            else
                pkg1.WriteString("Processo de busca sucedido! A sua equipe começará a guerra de sociedades!");
            game.SendToAll(pkg1, (IGamePlayer)null);
        }

        public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
        {
            GSPacketIn gsPacketIn = new GSPacketIn((short)186, player.Id);
            gsPacketIn.WriteInt(infos.Count);
            foreach (BufferInfo bufferInfo in infos)
            {
                gsPacketIn.WriteInt(bufferInfo.Type);
                gsPacketIn.WriteBoolean(bufferInfo.IsExist);
                gsPacketIn.WriteDateTime(bufferInfo.BeginDate);
                gsPacketIn.WriteInt(bufferInfo.ValidDate);
                gsPacketIn.WriteInt(bufferInfo.Value);
            }
            return gsPacketIn;
        }
    }
}
