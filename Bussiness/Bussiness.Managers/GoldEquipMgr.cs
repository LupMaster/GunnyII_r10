using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class GoldEquipMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, GoldEquipTemplateLoadInfo> _items;
        private static ReaderWriterLock m_lock;

        static GoldEquipMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, GoldEquipTemplateLoadInfo> infos = new Dictionary<int, GoldEquipTemplateLoadInfo>();
                if (GoldEquipMgr.LoadItem(infos))
                {
                    GoldEquipMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        GoldEquipMgr._items = infos;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        GoldEquipMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (GoldEquipMgr.log.IsErrorEnabled)
                    GoldEquipMgr.log.Error((object)"ReLoad", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                GoldEquipMgr.m_lock = new ReaderWriterLock();
                GoldEquipMgr._items = new Dictionary<int, GoldEquipTemplateLoadInfo>();
                flag = GoldEquipMgr.LoadItem(GoldEquipMgr._items);
            }
            catch (Exception ex)
            {
                if (GoldEquipMgr.log.IsErrorEnabled)
                    GoldEquipMgr.log.Error((object)"Init", ex);
                flag = false;
            }
            return flag;
        }

        public static bool LoadItem(Dictionary<int, GoldEquipTemplateLoadInfo> infos)
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (GoldEquipTemplateLoadInfo templateLoadInfo in produceBussiness.GetAllGoldEquipTemplateLoad())
                {
                    if (!Enumerable.Contains<int>((IEnumerable<int>)infos.Keys, templateLoadInfo.ID))
                        infos.Add(templateLoadInfo.ID, templateLoadInfo);
                }
            }
            return true;
        }

        public static GoldEquipTemplateLoadInfo FindGoldEquipCategoryID(int CategoryID)
        {
            if (GoldEquipMgr._items == null)
                GoldEquipMgr.Init();
            GoldEquipMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (GoldEquipTemplateLoadInfo templateLoadInfo in GoldEquipMgr._items.Values)
                {
                    if (templateLoadInfo.CategoryID == CategoryID)
                        return templateLoadInfo;
                }
            }
            finally
            {
                GoldEquipMgr.m_lock.ReleaseReaderLock();
            }
            return (GoldEquipTemplateLoadInfo)null;
        }

        public static GoldEquipTemplateLoadInfo FindGoldEquipNewTemplate(int TemplateId)
        {
            if (GoldEquipMgr._items == null)
                GoldEquipMgr.Init();
            GoldEquipMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (GoldEquipTemplateLoadInfo templateLoadInfo in GoldEquipMgr._items.Values)
                {
                    if (templateLoadInfo.OldTemplateId == TemplateId)
                        return templateLoadInfo;
                }
            }
            finally
            {
                GoldEquipMgr.m_lock.ReleaseReaderLock();
            }
            return (GoldEquipTemplateLoadInfo)null;
        }

        public static GoldEquipTemplateLoadInfo FindGoldEquipOldTemplate(int TemplateId)
        {
            if (GoldEquipMgr._items == null)
                GoldEquipMgr.Init();
            GoldEquipMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (GoldEquipTemplateLoadInfo templateLoadInfo in GoldEquipMgr._items.Values)
                {
                    if (templateLoadInfo.NewTemplateId == TemplateId && templateLoadInfo.OldTemplateId.ToString().Substring(4) != "4")
                        return templateLoadInfo;
                }
            }
            finally
            {
                GoldEquipMgr.m_lock.ReleaseReaderLock();
            }
            return (GoldEquipTemplateLoadInfo)null;
        }
    }
}
