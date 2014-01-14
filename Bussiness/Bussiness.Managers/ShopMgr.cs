using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public static class ShopMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();
        private static ReaderWriterLock m_lock = new ReaderWriterLock();

        static ShopMgr()
        {
        }

        public static bool Init()
        {
            return ShopMgr.ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ShopItemInfo> dictionary = ShopMgr.LoadFromDatabase();
                if (dictionary.Count > 0)
                    Interlocked.Exchange<Dictionary<int, ShopItemInfo>>(ref ShopMgr.m_shop, dictionary);
                return true;
            }
            catch (Exception ex)
            {
                ShopMgr.log.Error((object)"ShopInfoMgr", ex);
            }
            return false;
        }

        private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
        {
            Dictionary<int, ShopItemInfo> dictionary = new Dictionary<int, ShopItemInfo>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (ShopItemInfo shopItemInfo in produceBussiness.GetALllShop())
                {
                    if (!dictionary.ContainsKey(shopItemInfo.ID))
                        dictionary.Add(shopItemInfo.ID, shopItemInfo);
                }
            }
            return dictionary;
        }

        public static ShopItemInfo GetShopItemInfoById(int ID)
        {
            if (ShopMgr.m_shop.ContainsKey(ID))
                return ShopMgr.m_shop[ID];
            else
                return (ShopItemInfo)null;
        }

        public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
        {
            bool flag = false;
            using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
            {
                switch (shopID)
                {
                    case 1:
                        flag = true;
                        isBinds = false;
                        break;
                    case 2:
                        flag = true;
                        isBinds = false;
                        break;
                    case 3:
                        flag = true;
                        isBinds = false;
                        break;
                    case 4:
                        flag = true;
                        isBinds = false;
                        break;
                    case 11:
                        ConsortiaEquipControlInfo consortiaEuqipRiches1 = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
                        if (consortiaShopLevel >= consortiaEuqipRiches1.Level && playerRiches >= consortiaEuqipRiches1.Riches)
                        {
                            flag = true;
                            isBinds = true;
                            break;
                        }
                        else
                            break;
                    case 12:
                        ConsortiaEquipControlInfo consortiaEuqipRiches2 = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
                        if (consortiaShopLevel >= consortiaEuqipRiches2.Level && playerRiches >= consortiaEuqipRiches2.Riches)
                        {
                            flag = true;
                            isBinds = true;
                            break;
                        }
                        else
                            break;
                    case 13:
                        ConsortiaEquipControlInfo consortiaEuqipRiches3 = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
                        if (consortiaShopLevel >= consortiaEuqipRiches3.Level && playerRiches >= consortiaEuqipRiches3.Riches)
                        {
                            flag = true;
                            isBinds = true;
                            break;
                        }
                        else
                            break;
                    case 14:
                        ConsortiaEquipControlInfo consortiaEuqipRiches4 = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
                        if (consortiaShopLevel >= consortiaEuqipRiches4.Level && playerRiches >= consortiaEuqipRiches4.Riches)
                        {
                            flag = true;
                            isBinds = true;
                            break;
                        }
                        else
                            break;
                    case 15:
                        ConsortiaEquipControlInfo consortiaEuqipRiches5 = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
                        if (consortiaShopLevel >= consortiaEuqipRiches5.Level && playerRiches >= consortiaEuqipRiches5.Riches)
                        {
                            flag = true;
                            isBinds = true;
                            break;
                        }
                        else
                            break;
                }
            }
            return flag;
        }

        public static int FindItemTemplateID(int id)
        {
            if (ShopMgr.m_shop.ContainsKey(id))
                return ShopMgr.m_shop[id].TemplateID;
            else
                return 0;
        }

        public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
        {
            List<ShopItemInfo> list = new List<ShopItemInfo>();
            foreach (ShopItemInfo shopItemInfo in ShopMgr.m_shop.Values)
            {
                if (shopItemInfo.TemplateID == TemplatID)
                    list.Add(shopItemInfo);
            }
            return list;
        }

        public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
        {
            foreach (ShopItemInfo shopItemInfo in ShopMgr.m_shop.Values)
            {
                if (shopItemInfo.TemplateID == TemplatID)
                    return shopItemInfo;
            }
            return (ShopItemInfo)null;
        }
    }
}
