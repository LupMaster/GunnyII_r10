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
    public class ItemMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, ItemTemplateInfo> _items;
        private static Dictionary<int, LoadUserBoxInfo> _timeBoxs;
        private static List<ItemTemplateInfo> Lists;
        private static ReaderWriterLock m_lock;

        static ItemMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ItemTemplateInfo> infos = new Dictionary<int, ItemTemplateInfo>();
                Dictionary<int, LoadUserBoxInfo> userBoxs = new Dictionary<int, LoadUserBoxInfo>();
                if (ItemMgr.LoadItem(infos, userBoxs))
                {
                    ItemMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        ItemMgr._items = infos;
                        ItemMgr._timeBoxs = userBoxs;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ItemMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ItemMgr.log.IsErrorEnabled)
                    ItemMgr.log.Error((object)"ReLoad", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                ItemMgr.m_lock = new ReaderWriterLock();
                ItemMgr._items = new Dictionary<int, ItemTemplateInfo>();
                ItemMgr._timeBoxs = new Dictionary<int, LoadUserBoxInfo>();
                ItemMgr.Lists = new List<ItemTemplateInfo>();
                flag = ItemMgr.LoadItem(ItemMgr._items, ItemMgr._timeBoxs);
            }
            catch (Exception ex)
            {
                if (ItemMgr.log.IsErrorEnabled)
                    ItemMgr.log.Error((object)"Init", ex);
                flag = false;
            }
            return flag;
        }

        public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos, Dictionary<int, LoadUserBoxInfo> userBoxs)
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (ItemTemplateInfo itemTemplateInfo in produceBussiness.GetAllGoods())
                {
                    if (!Enumerable.Contains<int>((IEnumerable<int>)infos.Keys, itemTemplateInfo.TemplateID))
                        infos.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
                }
                foreach (LoadUserBoxInfo loadUserBoxInfo in produceBussiness.GetAllTimeBoxAward())
                {
                    if (!Enumerable.Contains<int>((IEnumerable<int>)infos.Keys, loadUserBoxInfo.ID))
                        userBoxs.Add(loadUserBoxInfo.ID, loadUserBoxInfo);
                }
            }
            return true;
        }

        public static LoadUserBoxInfo FindItemBoxTypeAndLv(int type, int lv)
        {
            if (ItemMgr._timeBoxs == null)
                ItemMgr.Init();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (LoadUserBoxInfo loadUserBoxInfo in ItemMgr._timeBoxs.Values)
                {
                    if (loadUserBoxInfo.Type == type && loadUserBoxInfo.Level == lv)
                        return loadUserBoxInfo;
                }
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return (LoadUserBoxInfo)null;
        }

        public static LoadUserBoxInfo FindItemBoxTemplate(int Id)
        {
            if (ItemMgr._timeBoxs == null)
                ItemMgr.Init();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (Enumerable.Contains<int>((IEnumerable<int>)ItemMgr._timeBoxs.Keys, Id))
                    return ItemMgr._timeBoxs[Id];
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return (LoadUserBoxInfo)null;
        }

        public static void LoadRecycleItemTemplate()
        {
            if (ItemMgr._items == null)
                ItemMgr.Init();
            foreach (ItemTemplateInfo itemTemplateInfo in ItemMgr._items.Values)
            {
                if (itemTemplateInfo.Quality > 1 && itemTemplateInfo.Quality < 5 && (itemTemplateInfo.CanRecycle > 0 && itemTemplateInfo.TemplateID > 0) && itemTemplateInfo.TemplateID != 11107)
                {
                    switch (itemTemplateInfo.CategoryID)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 8:
                        case 9:
                        case 11:
                        case 13:
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                            ItemMgr.Lists.Add(itemTemplateInfo);
                            break;
                    }
                }
            }
        }

        public static List<ItemTemplateInfo> FindRecycleItemTemplate(int qmax)
        {
            if (ItemMgr.Lists.Count == 0)
                ItemMgr.LoadRecycleItemTemplate();
            List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (ItemTemplateInfo itemTemplateInfo in ItemMgr.Lists)
                {
                    if (itemTemplateInfo.Quality < qmax)
                        list.Add(itemTemplateInfo);
                }
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return list;
        }

        public static ItemTemplateInfo FindItemTemplate(int templateId)
        {
            if (ItemMgr._items == null)
                ItemMgr.Init();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (Enumerable.Contains<int>((IEnumerable<int>)ItemMgr._items.Keys, templateId))
                    return ItemMgr._items[templateId];
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return (ItemTemplateInfo)null;
        }

        public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
        {
            if (ItemMgr._items == null)
                ItemMgr.Init();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (ItemTemplateInfo itemTemplateInfo in ItemMgr._items.Values)
                {
                    if (itemTemplateInfo.FusionType == fusionType && itemTemplateInfo.Quality == quality)
                        return itemTemplateInfo;
                }
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return (ItemTemplateInfo)null;
        }

        public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
        {
            if (ItemMgr._items == null)
                ItemMgr.Init();
            ItemMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                foreach (ItemTemplateInfo itemTemplateInfo in ItemMgr._items.Values)
                {
                    if (itemTemplateInfo.FusionType == fusionType && itemTemplateInfo.Level == level)
                        return itemTemplateInfo;
                }
            }
            finally
            {
                ItemMgr.m_lock.ReleaseReaderLock();
            }
            return (ItemTemplateInfo)null;
        }

        public static List<SqlDataProvider.Data.ItemInfo> SpiltGoodsMaxCount(SqlDataProvider.Data.ItemInfo itemInfo)
        {
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            int num1 = 0;
            while (num1 < itemInfo.Count)
            {
                int num2 = itemInfo.Count < itemInfo.Template.MaxCount ? itemInfo.Count : itemInfo.Template.MaxCount;
                SqlDataProvider.Data.ItemInfo itemInfo1 = itemInfo.Clone();
                itemInfo1.Count = num2;
                list.Add(itemInfo1);
                num1 += itemInfo.Template.MaxCount;
            }
            return list;
        }
    }
}
