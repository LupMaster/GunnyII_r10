using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Center.Server
{
    public class ServerMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();
        private static object _syncStop = new object();

        public static ServerInfo[] Servers
        {
            get
            {
                return Enumerable.ToArray<ServerInfo>((IEnumerable<ServerInfo>)ServerMgr._list.Values);
            }
        }

        static ServerMgr()
        {
        }

        public static bool Start()
        {
            bool flag;
            try
            {
                using (ServiceBussiness serviceBussiness = new ServiceBussiness())
                {
                    foreach (ServerInfo serverInfo in serviceBussiness.GetServerList())
                    {
                        serverInfo.State = 1;
                        serverInfo.Online = 0;
                        ServerMgr._list.Add(serverInfo.ID, serverInfo);
                    }
                }
                ServerMgr.log.Info((object)"Load server list from db.");
                flag = true;
            }
            catch (Exception ex)
            {
                ServerMgr.log.ErrorFormat("Load server list from db failed:{0}", (object)ex);
                flag = false;
            }
            return flag;
        }

        public static bool ReLoadServerList()
        {
            bool flag;
            try
            {
                using (ServiceBussiness serviceBussiness = new ServiceBussiness())
                {
                    lock (ServerMgr._syncStop)
                    {
                        foreach (ServerInfo item_0 in serviceBussiness.GetServerList())
                        {
                            if (ServerMgr._list.ContainsKey(item_0.ID))
                            {
                                ServerMgr._list[item_0.ID].IP = item_0.IP;
                                ServerMgr._list[item_0.ID].Name = item_0.Name;
                                ServerMgr._list[item_0.ID].Port = item_0.Port;
                                ServerMgr._list[item_0.ID].Room = item_0.Room;
                                ServerMgr._list[item_0.ID].Total = item_0.Total;
                                ServerMgr._list[item_0.ID].MustLevel = item_0.MustLevel;
                                ServerMgr._list[item_0.ID].LowestLevel = item_0.LowestLevel;
                                ServerMgr._list[item_0.ID].Online = item_0.Online;
                                ServerMgr._list[item_0.ID].State = item_0.State;
                            }
                            else
                            {
                                item_0.State = 1;
                                item_0.Online = 0;
                                ServerMgr._list.Add(item_0.ID, item_0);
                            }
                        }
                    }
                }
                ServerMgr.log.Info((object)"ReLoad server list from db.");
                flag = true;
            }
            catch (Exception ex)
            {
                ServerMgr.log.ErrorFormat("ReLoad server list from db failed:{0}", (object)ex);
                flag = false;
            }
            return flag;
        }

        public static ServerInfo GetServerInfo(int id)
        {
            if (ServerMgr._list.ContainsKey(id))
                return ServerMgr._list[id];
            else
                return (ServerInfo)null;
        }

        public static int GetState(int count, int total)
        {
            if (count >= total)
                return 5;
            return (double)count > (double)total * 0.5 ? 4 : 2;
        }

        public static void SaveToDatabase()
        {
            try
            {
                using (ServiceBussiness serviceBussiness = new ServiceBussiness())
                {
                    foreach (ServerInfo info in ServerMgr._list.Values)
                        serviceBussiness.UpdateService(info);
                }
            }
            catch (Exception ex)
            {
                ServerMgr.log.Error((object)"Save server state", ex);
            }
        }
    }
}
