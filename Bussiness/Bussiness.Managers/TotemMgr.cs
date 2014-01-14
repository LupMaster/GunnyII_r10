using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class TotemMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, TotemInfo> _totem;
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        static TotemMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TotemInfo> totem = new Dictionary<int, TotemInfo>();
                if (TotemMgr.Load(totem))
                {
                    TotemMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        TotemMgr._totem = totem;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        TotemMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (TotemMgr.log.IsErrorEnabled)
                    TotemMgr.log.Error((object)"TotemMgr", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                TotemMgr.m_lock = new ReaderWriterLock();
                TotemMgr._totem = new Dictionary<int, TotemInfo>();
                TotemMgr.rand = new ThreadSafeRandom();
                flag = TotemMgr.Load(TotemMgr._totem);
            }
            catch (Exception ex)
            {
                if (TotemMgr.log.IsErrorEnabled)
                    TotemMgr.log.Error((object)"TotemMgr", ex);
                flag = false;
            }
            return flag;
        }

        private static bool Load(Dictionary<int, TotemInfo> totem)
        {
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                foreach (TotemInfo totemInfo in playerBussiness.GetAllTotem())
                {
                    if (!totem.ContainsKey(totemInfo.ID))
                        totem.Add(totemInfo.ID, totemInfo);
                }
            }
            return true;
        }

        public static TotemInfo FindTotemInfo(int ID)
        {
            if (ID < 10000)
                ID = 10001;
            TotemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (TotemMgr._totem.ContainsKey(ID))
                    return TotemMgr._totem[ID];
            }
            catch
            {
            }
            finally
            {
                TotemMgr.m_lock.ReleaseReaderLock();
            }
            return (TotemInfo)null;
        }

        public static int GetTotemProp(int id, string typeOf)
        {
            int num = 0;
            for (int ID = 10001; ID <= id; ++ID)
            {
                TotemInfo totemInfo = TotemMgr.FindTotemInfo(ID);
                switch (typeOf)
                {
                    case "att":
                        num += totemInfo.AddAttack;
                        break;
                    case "agi":
                        num += totemInfo.AddAgility;
                        break;
                    case "def":
                        num += totemInfo.AddDefence;
                        break;
                    case "luc":
                        num += totemInfo.AddLuck;
                        break;
                    case "blo":
                        num += totemInfo.AddBlood;
                        break;
                    case "dam":
                        num += totemInfo.AddDamage;
                        break;
                    case "gua":
                        num += totemInfo.AddGuard;
                        break;
                }
            }
            return num;
        }
    }
}
