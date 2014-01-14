using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class FightSpiritTemplateMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, FightSpiritTemplateInfo> _fightSpiritTemplate;
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        static FightSpiritTemplateMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, FightSpiritTemplateInfo> consortiaLevel = new Dictionary<int, FightSpiritTemplateInfo>();
                if (FightSpiritTemplateMgr.Load(consortiaLevel))
                {
                    FightSpiritTemplateMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        FightSpiritTemplateMgr._fightSpiritTemplate = consortiaLevel;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        FightSpiritTemplateMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (FightSpiritTemplateMgr.log.IsErrorEnabled)
                    FightSpiritTemplateMgr.log.Error((object)"ConsortiaLevelMgr", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                FightSpiritTemplateMgr.m_lock = new ReaderWriterLock();
                FightSpiritTemplateMgr._fightSpiritTemplate = new Dictionary<int, FightSpiritTemplateInfo>();
                FightSpiritTemplateMgr.rand = new ThreadSafeRandom();
                flag = FightSpiritTemplateMgr.Load(FightSpiritTemplateMgr._fightSpiritTemplate);
            }
            catch (Exception ex)
            {
                if (FightSpiritTemplateMgr.log.IsErrorEnabled)
                    FightSpiritTemplateMgr.log.Error((object)"ConsortiaLevelMgr", ex);
                flag = false;
            }
            return flag;
        }

        private static bool Load(Dictionary<int, FightSpiritTemplateInfo> consortiaLevel)
        {
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                foreach (FightSpiritTemplateInfo spiritTemplateInfo in playerBussiness.GetAllFightSpiritTemplate())
                {
                    if (!consortiaLevel.ContainsKey(spiritTemplateInfo.ID))
                        consortiaLevel.Add(spiritTemplateInfo.ID, spiritTemplateInfo);
                }
            }
            return true;
        }

        public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
        {
            FightSpiritTemplateMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (FightSpiritTemplateInfo spiritTemplateInfo in FightSpiritTemplateMgr._fightSpiritTemplate.Values)
                {
                    if (spiritTemplateInfo.FightSpiritID == FigSpiritId && spiritTemplateInfo.Level == lv)
                        return spiritTemplateInfo;
                }
            }
            catch
            {
            }
            finally
            {
                FightSpiritTemplateMgr.m_lock.ReleaseReaderLock();
            }
            return (FightSpiritTemplateInfo)null;
        }

        public static int getProp(int FigSpiritId, int lv, int place)
        {
            FightSpiritTemplateInfo spiritTemplateInfo = FightSpiritTemplateMgr.FindFightSpiritTemplateInfo(FigSpiritId, lv);
            switch (place)
            {
                case 2:
                    return spiritTemplateInfo.Attack;
                case 3:
                    return spiritTemplateInfo.Lucky;
                case 5:
                    return spiritTemplateInfo.Agility;
                case 11:
                    return spiritTemplateInfo.Defence;
                case 13:
                    return spiritTemplateInfo.Blood;
                default:
                    return 0;
            }
        }
    }
}
