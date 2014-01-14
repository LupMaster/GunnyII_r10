using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class TotemHonorMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, TotemHonorTemplateInfo> _totemHonorTemplate;
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        static TotemHonorMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
                if (TotemHonorMgr.Load(TotemHonorTemplate))
                {
                    TotemHonorMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        TotemHonorMgr._totemHonorTemplate = TotemHonorTemplate;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        TotemHonorMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (TotemHonorMgr.log.IsErrorEnabled)
                    TotemHonorMgr.log.Error((object)"ConsortiaLevelMgr", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                TotemHonorMgr.m_lock = new ReaderWriterLock();
                TotemHonorMgr._totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
                TotemHonorMgr.rand = new ThreadSafeRandom();
                flag = TotemHonorMgr.Load(TotemHonorMgr._totemHonorTemplate);
            }
            catch (Exception ex)
            {
                if (TotemHonorMgr.log.IsErrorEnabled)
                    TotemHonorMgr.log.Error((object)"ConsortiaLevelMgr", ex);
                flag = false;
            }
            return flag;
        }

        private static bool Load(Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate)
        {
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                foreach (TotemHonorTemplateInfo honorTemplateInfo in playerBussiness.GetAllTotemHonorTemplate())
                {
                    if (!TotemHonorTemplate.ContainsKey(honorTemplateInfo.ID))
                        TotemHonorTemplate.Add(honorTemplateInfo.ID, honorTemplateInfo);
                }
            }
            return true;
        }

        public static TotemHonorTemplateInfo FindTotemHonorTemplateInfo(int ID)
        {
            TotemHonorMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (TotemHonorMgr._totemHonorTemplate.ContainsKey(ID))
                    return TotemHonorMgr._totemHonorTemplate[ID];
            }
            catch
            {
            }
            finally
            {
                TotemHonorMgr.m_lock.ReleaseReaderLock();
            }
            return (TotemHonorTemplateInfo)null;
        }
    }
}
