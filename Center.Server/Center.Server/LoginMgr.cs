using System;
using System.Collections.Generic;
using System.Linq;

namespace Center.Server
{
    public class LoginMgr
    {
        private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();
        private static object syc_obj = new object();

        static LoginMgr()
        {
        }

        public static void CreatePlayer(Player player)
        {
            Player player1 = (Player)null;
            lock (LoginMgr.syc_obj)
            {
                player.LastTime = DateTime.Now.Ticks;
                if (LoginMgr.m_players.ContainsKey(player.Id))
                {
                    player1 = LoginMgr.m_players[player.Id];
                    player.State = player1.State;
                    player.CurrentServer = player1.CurrentServer;
                    LoginMgr.m_players[player.Id] = player;
                }
                else
                {
                    player1 = LoginMgr.GetPlayerByName(player.Name);
                    if (player1 != null && LoginMgr.m_players.ContainsKey(player1.Id))
                        LoginMgr.m_players.Remove(player1.Id);
                    player.State = ePlayerState.NotLogin;
                    LoginMgr.m_players.Add(player.Id, player);
                }
            }
            if (player1 == null || player1.CurrentServer == null)
                return;
            player1.CurrentServer.SendKitoffUser(player1.Id);
        }

        public static bool TryLoginPlayer(int id, ServerClient server)
        {
            bool flag;
            lock (LoginMgr.syc_obj)
            {
                if (LoginMgr.m_players.ContainsKey(id))
                {
                    Player local_2 = LoginMgr.m_players[id];
                    if (local_2.CurrentServer == null)
                    {
                        local_2.CurrentServer = server;
                        local_2.State = ePlayerState.Logining;
                        flag = true;
                    }
                    else
                    {
                        if (local_2.State == ePlayerState.Play)
                            local_2.CurrentServer.SendKitoffUser(id);
                        flag = false;
                    }
                }
                else
                    flag = false;
            }
            return flag;
        }

        public static void PlayerLogined(int id, ServerClient server)
        {
            lock (LoginMgr.syc_obj)
            {
                if (!LoginMgr.m_players.ContainsKey(id))
                    return;
                Player local_1 = LoginMgr.m_players[id];
                if (local_1 != null)
                {
                    local_1.CurrentServer = server;
                    local_1.State = ePlayerState.Play;
                }
            }
        }

        public static void PlayerLoginOut(int id, ServerClient server)
        {
            lock (LoginMgr.syc_obj)
            {
                if (!LoginMgr.m_players.ContainsKey(id))
                    return;
                Player local_1 = LoginMgr.m_players[id];
                if (local_1 != null && local_1.CurrentServer == server)
                {
                    local_1.CurrentServer = (ServerClient)null;
                    local_1.State = ePlayerState.NotLogin;
                }
            }
        }

        public static Player GetPlayerByName(string name)
        {
            Player[] allPlayer = LoginMgr.GetAllPlayer();
            if (allPlayer != null)
            {
                foreach (Player player in allPlayer)
                {
                    if (player.Name == name)
                        return player;
                }
            }
            return (Player)null;
        }

        public static Player[] GetAllPlayer()
        {
            Player[] playerArray;
            lock (LoginMgr.syc_obj)
                playerArray = Enumerable.ToArray<Player>((IEnumerable<Player>)LoginMgr.m_players.Values);
            return playerArray;
        }

        public static void RemovePlayer(int playerId)
        {
            lock (LoginMgr.syc_obj)
            {
                if (!LoginMgr.m_players.ContainsKey(playerId))
                    return;
                LoginMgr.m_players.Remove(playerId);
            }
        }

        public static void RemovePlayer(List<Player> players)
        {
            lock (LoginMgr.syc_obj)
            {
                foreach (Player item_0 in players)
                    LoginMgr.m_players.Remove(item_0.Id);
            }
        }

        public static Player GetPlayer(int playerId)
        {
            lock (LoginMgr.syc_obj)
            {
                if (LoginMgr.m_players.ContainsKey(playerId))
                    return LoginMgr.m_players[playerId];
            }
            return (Player)null;
        }

        public static ServerClient GetServerClient(int playerId)
        {
            Player player = LoginMgr.GetPlayer(playerId);
            if (player != null)
                return player.CurrentServer;
            else
                return (ServerClient)null;
        }

        public static int GetOnlineCount()
        {
            Player[] allPlayer = LoginMgr.GetAllPlayer();
            int num = 0;
            foreach (Player player in allPlayer)
            {
                if (player.State != ePlayerState.NotLogin)
                    ++num;
            }
            return num;
        }

        public static Dictionary<int, int> GetOnlineForLine()
        {
            Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
            foreach (Player player in LoginMgr.GetAllPlayer())
            {
                if (player.CurrentServer != null)
                {
                    if (dictionary1.ContainsKey(player.CurrentServer.Info.ID))
                    {
                        Dictionary<int, int> dictionary2;
                        int id;
                        (dictionary2 = dictionary1)[id = player.CurrentServer.Info.ID] = dictionary2[id] + 1;
                    }
                    else
                        dictionary1.Add(player.CurrentServer.Info.ID, 1);
                }
            }
            return dictionary1;
        }

        public static List<Player> GetServerPlayers(ServerClient server)
        {
            List<Player> list = new List<Player>();
            foreach (Player player in LoginMgr.GetAllPlayer())
            {
                if (player.CurrentServer == server)
                    list.Add(player);
            }
            return list;
        }
    }
}
