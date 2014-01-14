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
    public class ItemBoxMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ThreadSafeRandom random = new ThreadSafeRandom();
        private static ItemBoxInfo[] m_itemBox;
        private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;

        static ItemBoxMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                ItemBoxInfo[] itemBoxs = ItemBoxMgr.LoadItemBoxDb();
                Dictionary<int, List<ItemBoxInfo>> dictionary = ItemBoxMgr.LoadItemBoxs(itemBoxs);
                if (itemBoxs != null)
                {
                    Interlocked.Exchange<ItemBoxInfo[]>(ref ItemBoxMgr.m_itemBox, itemBoxs);
                    Interlocked.Exchange<Dictionary<int, List<ItemBoxInfo>>>(ref ItemBoxMgr.m_itemBoxs, dictionary);
                }
            }
            catch (Exception ex)
            {
                if (ItemBoxMgr.log.IsErrorEnabled)
                    ItemBoxMgr.log.Error((object)"ReLoad", ex);
                return false;
            }
            return true;
        }

        public static bool Init()
        {
            return ItemBoxMgr.ReLoad();
        }

        public static ItemBoxInfo[] LoadItemBoxDb()
        {
            ItemBoxInfo[] itemBoxInfos;
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
                itemBoxInfos = produceBussiness.GetItemBoxInfos();
            return itemBoxInfos;
        }

        public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
        {
            Dictionary<int, List<ItemBoxInfo>> dictionary = new Dictionary<int, List<ItemBoxInfo>>();
            for (int index = 0; index < itemBoxs.Length; ++index)
            {
                ItemBoxInfo info = itemBoxs[index];
                if (!Enumerable.Contains<int>((IEnumerable<int>)dictionary.Keys, info.DataId))
                {
                    IEnumerable<ItemBoxInfo> source = Enumerable.Where<ItemBoxInfo>((IEnumerable<ItemBoxInfo>)itemBoxs, (Func<ItemBoxInfo, bool>)(s => s.DataId == info.DataId));
                    dictionary.Add(info.DataId, Enumerable.ToList<ItemBoxInfo>(source));
                }
            }
            return dictionary;
        }

        public static List<ItemBoxInfo> FindItemBox(int DataId)
        {
            if (ItemBoxMgr.m_itemBoxs.ContainsKey(DataId))
                return ItemBoxMgr.m_itemBoxs[DataId];
            else
                return (List<ItemBoxInfo>)null;
        }

        public static List<SqlDataProvider.Data.ItemInfo> GetAllItemBoxAward(int DataId)
        {
            List<ItemBoxInfo> itemBox = ItemBoxMgr.FindItemBox(DataId);
            List<SqlDataProvider.Data.ItemInfo> list = new List<SqlDataProvider.Data.ItemInfo>();
            foreach (ItemBoxInfo itemBoxInfo in itemBox)
            {
                SqlDataProvider.Data.ItemInfo fromTemplate = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(itemBoxInfo.TemplateId), itemBoxInfo.ItemCount, 105);
                fromTemplate.IsBinds = itemBoxInfo.IsBind;
                fromTemplate.ValidDate = itemBoxInfo.ItemValid;
                list.Add(fromTemplate);
            }
            return list;
        }

        public static ItemBoxInfo FindSpecialItemBox(int DataId)
        {
            ItemBoxInfo itemBoxInfo = new ItemBoxInfo();
            if (DataId <= -300)
            {
                if (DataId != -1100)
                {
                    if (DataId == -300)
                    {
                        itemBoxInfo.TemplateId = 11420;
                        itemBoxInfo.ItemCount = 1;
                    }
                }
                else
                {
                    itemBoxInfo.TemplateId = 11213;
                    itemBoxInfo.ItemCount = 1;
                }
            }
            else if (DataId != -200)
            {
                if (DataId != -100)
                {
                    if (DataId == 11408)
                    {
                        itemBoxInfo.TemplateId = 11420;
                        itemBoxInfo.ItemCount = 1;
                    }
                }
                else
                {
                    itemBoxInfo.TemplateId = 11233;
                    itemBoxInfo.ItemCount = 1;
                }
            }
            else
            {
                itemBoxInfo.TemplateId = 112244;
                itemBoxInfo.ItemCount = 1;
            }
            return itemBoxInfo;
        }

        public static bool CreateItemBox(int DateId, List<SqlDataProvider.Data.ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal, ref int exp)
        {
            List<ItemBoxInfo> list1 = new List<ItemBoxInfo>();
            List<ItemBoxInfo> itemBox = ItemBoxMgr.FindItemBox(DateId);
            if (itemBox == null)
                return false;
            List<ItemBoxInfo> list2 = Enumerable.ToList<ItemBoxInfo>(Enumerable.Where<ItemBoxInfo>((IEnumerable<ItemBoxInfo>)itemBox, (Func<ItemBoxInfo, bool>)(s => s.IsSelect)));
            int num1 = 1;
            int maxRound = 0;
            if (list2.Count < itemBox.Count)
                maxRound = ThreadSafeRandom.NextStatic(Enumerable.Max(Enumerable.Select<ItemBoxInfo, int>(Enumerable.Where<ItemBoxInfo>((IEnumerable<ItemBoxInfo>)itemBox, (Func<ItemBoxInfo, bool>)(s => !s.IsSelect)), (Func<ItemBoxInfo, int>)(s => s.Random))));
            List<ItemBoxInfo> list3 = Enumerable.ToList<ItemBoxInfo>(Enumerable.Where<ItemBoxInfo>((IEnumerable<ItemBoxInfo>)itemBox, (Func<ItemBoxInfo, bool>)(s => !s.IsSelect && s.Random >= maxRound)));
            int num2 = Enumerable.Count<ItemBoxInfo>((IEnumerable<ItemBoxInfo>)list3);
            if (num2 > 0)
            {
                int count = num1 > num2 ? num2 : num1;
                foreach (int index in ItemBoxMgr.GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    ItemBoxInfo itemBoxInfo = list3[index];
                    if (list2 == null)
                        list2 = new List<ItemBoxInfo>();
                    list2.Add(itemBoxInfo);
                }
            }
            foreach (ItemBoxInfo itemBoxInfo in list2)
            {
                if (itemBoxInfo == null)
                    return false;
                int templateId = itemBoxInfo.TemplateId;
                if (templateId <= -900)
                {
                    if (templateId <= -1100)
                    {
                        if (templateId == -1200 || templateId == -1100)
                        {
                            giftToken += itemBoxInfo.ItemCount;
                            continue;
                        }
                    }
                    else if (templateId == -1000 || templateId == -900)
                        continue;
                }
                else if (templateId <= -300)
                {
                    if (templateId != -800)
                    {
                        if (templateId == -300)
                        {
                            medal += itemBoxInfo.ItemCount;
                            continue;
                        }
                    }
                    else
                        continue;
                }
                else if (templateId == -200)
                {
                    point += itemBoxInfo.ItemCount;
                    continue;
                }
                else if (templateId == -100)
                {
                    gold += itemBoxInfo.ItemCount;
                    continue;
                }
                else if (templateId == 11107)
                {
                    exp += itemBoxInfo.ItemCount;
                    continue;
                }
                SqlDataProvider.Data.ItemInfo fromTemplate = SqlDataProvider.Data.ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(itemBoxInfo.TemplateId), itemBoxInfo.ItemCount, 101);
                if (fromTemplate != null)
                {
                    fromTemplate.Count = itemBoxInfo.ItemCount;
                    fromTemplate.IsBinds = itemBoxInfo.IsBind;
                    fromTemplate.ValidDate = itemBoxInfo.ItemValid;
                    fromTemplate.StrengthenLevel = itemBoxInfo.StrengthenLevel;
                    fromTemplate.AttackCompose = itemBoxInfo.AttackCompose;
                    fromTemplate.DefendCompose = itemBoxInfo.DefendCompose;
                    fromTemplate.AgilityCompose = itemBoxInfo.AgilityCompose;
                    fromTemplate.LuckCompose = itemBoxInfo.LuckCompose;
                    fromTemplate.IsTips = itemBoxInfo.IsTips != 0;
                    fromTemplate.IsLogs = itemBoxInfo.IsLogs;
                    if (itemInfos == null)
                        itemInfos = new List<SqlDataProvider.Data.ItemInfo>();
                    itemInfos.Add(fromTemplate);
                }
            }
            return true;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int index1 = 0; index1 < count; ++index1)
            {
                int num1 = ItemBoxMgr.random.Next(minValue, maxValue + 1);
                int num2 = 0;
                for (int index2 = 0; index2 < index1; ++index2)
                {
                    if (numArray[index2] == num1)
                        ++num2;
                }
                if (num2 == 0)
                    numArray[index1] = num1;
                else
                    --index1;
            }
            return numArray;
        }
    }
}
