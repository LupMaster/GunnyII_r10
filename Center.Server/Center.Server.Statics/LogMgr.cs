using Bussiness;
using Center.Server;
using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace Center.Server.Statics
{
    public class LogMgr
    {
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static object _syncStop = new object();
        public static object _sysObj = new object();
        private static int _gameType;
        private static int _serverId;
        private static int _areaId;
        public static DataTable m_LogServer;
        private static int regCount;

        public static int GameType
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["GameType"]);
            }
        }

        public static int ServerID
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["ServerID"]);
            }
        }

        public static int AreaID
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["AreaID"]);
            }
        }

        public static int SaveRecordSecond
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["SaveRecordInterval"]) * 60;
            }
        }

        public static int RegCount
        {
            get
            {
                int num;
                lock (LogMgr._sysObj)
                    num = LogMgr.regCount;
                return num;
            }
            set
            {
                lock (LogMgr._sysObj)
                    LogMgr.regCount = value;
            }
        }

        static LogMgr()
        {
        }

        public static bool Setup()
        {
            return LogMgr.Setup(LogMgr.GameType, LogMgr.ServerID, LogMgr.AreaID);
        }

        public static bool Setup(int gametype, int serverid, int areaid)
        {
            LogMgr._gameType = gametype;
            LogMgr._serverId = serverid;
            LogMgr._areaId = areaid;
            LogMgr.m_LogServer = new DataTable("Log_Server");
            LogMgr.m_LogServer.Columns.Add("ApplicationId", typeof(int));
            LogMgr.m_LogServer.Columns.Add("SubId", typeof(int));
            LogMgr.m_LogServer.Columns.Add("EnterTime", typeof(DateTime));
            LogMgr.m_LogServer.Columns.Add("Online", typeof(int));
            LogMgr.m_LogServer.Columns.Add("Reg", typeof(int));
            return true;
        }

        public static void Reset()
        {
            lock (LogMgr.m_LogServer)
                LogMgr.m_LogServer.Clear();
        }

        public static void Save()
        {
            LoginMgr.GetOnlineCount();
            int num1 = LogMgr._gameType;
            int num2 = LogMgr._serverId;
            DateTime now = DateTime.Now;
            int regCount = LogMgr.RegCount;
            LogMgr.RegCount = 0;
            int saveRecordSecond = LogMgr.SaveRecordSecond;
            using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
                itemRecordBussiness.LogServerDb(LogMgr.m_LogServer);
        }

        public static void AddRegCount()
        {
            lock (LogMgr._sysObj)
                ++LogMgr.regCount;
        }
    }
}
