using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Game.Server.Managers
{
    public class ConsortiaLevelMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        static ConsortiaLevelMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ConsortiaLevelInfo> consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
                if (ConsortiaLevelMgr.Load(consortiaLevel))
                {
                    ConsortiaLevelMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        ConsortiaLevelMgr._consortiaLevel = consortiaLevel;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ConsortiaLevelMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ConsortiaLevelMgr.log.IsErrorEnabled)
                    ConsortiaLevelMgr.log.Error((object)"ConsortiaLevelMgr", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                ConsortiaLevelMgr.m_lock = new ReaderWriterLock();
                ConsortiaLevelMgr._consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
                ConsortiaLevelMgr.rand = new ThreadSafeRandom();
                flag = ConsortiaLevelMgr.Load(ConsortiaLevelMgr._consortiaLevel);
            }
            catch (Exception ex)
            {
                if (ConsortiaLevelMgr.log.IsErrorEnabled)
                    ConsortiaLevelMgr.log.Error((object)"ConsortiaLevelMgr", ex);
                flag = false;
            }
            return flag;
        }

        private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel)
        {
            using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
            {
                foreach (ConsortiaLevelInfo consortiaLevelInfo in consortiaBussiness.GetAllConsortiaLevel())
                {
                    if (!consortiaLevel.ContainsKey(consortiaLevelInfo.Level))
                        consortiaLevel.Add(consortiaLevelInfo.Level, consortiaLevelInfo);
                }
            }
            return true;
        }

        public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
        {
            ConsortiaLevelMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (ConsortiaLevelMgr._consortiaLevel.ContainsKey(level))
                    return ConsortiaLevelMgr._consortiaLevel[level];
            }
            catch
            {
            }
            finally
            {
                ConsortiaLevelMgr.m_lock.ReleaseReaderLock();
            }
            return (ConsortiaLevelInfo)null;
        }
    }
}
