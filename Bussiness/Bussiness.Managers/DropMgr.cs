using Bussiness;
using Bussiness.Protocol;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class DropMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));
        private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();
        private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();

        static DropMgr()
        {
        }

        public static bool Init()
        {
            return DropMgr.ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                List<DropCondiction> list = DropMgr.LoadDropConditionDb();
                Interlocked.Exchange<List<DropCondiction>>(ref DropMgr.m_dropcondiction, list);
                Dictionary<int, List<DropItem>> dictionary = DropMgr.LoadDropItemDb();
                Interlocked.Exchange<Dictionary<int, List<DropItem>>>(ref DropMgr.m_dropitem, dictionary);
                return true;
            }
            catch (Exception ex)
            {
                DropMgr.log.Error((object)"DropMgr", ex);
            }
            return false;
        }

        public static List<DropCondiction> LoadDropConditionDb()
        {
            List<DropCondiction> list;
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                DropCondiction[] allDropCondictions = produceBussiness.GetAllDropCondictions();
                list = allDropCondictions != null ? Enumerable.ToList<DropCondiction>((IEnumerable<DropCondiction>)allDropCondictions) : (List<DropCondiction>)null;
            }
            return list;
        }

        public static Dictionary<int, List<DropItem>> LoadDropItemDb()
        {
            Dictionary<int, List<DropItem>> dictionary = new Dictionary<int, List<DropItem>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                DropItem[] allDropItems = produceBussiness.GetAllDropItems();
                foreach (DropCondiction dropCondiction in DropMgr.m_dropcondiction)
                {
                    DropCondiction info = dropCondiction;
                    IEnumerable<DropItem> source = Enumerable.Where<DropItem>((IEnumerable<DropItem>)allDropItems, (Func<DropItem, bool>)(s => s.DropId == info.DropId));
                    dictionary.Add(info.DropId, Enumerable.ToList<DropItem>(source));
                }
            }
            return dictionary;
        }

        public static int FindCondiction(eDropType type, string para1, string para2)
        {
            string str1 = "," + para1 + ",";
            string str2 = "," + para2 + ",";
            foreach (DropCondiction dropCondiction in DropMgr.m_dropcondiction)
            {
                if ((eDropType)dropCondiction.CondictionType == type && dropCondiction.Para1.IndexOf(str1) != -1 && dropCondiction.Para2.IndexOf(str2) != -1)
                    return dropCondiction.DropId;
            }
            return 0;
        }

        public static List<DropItem> FindDropItem(int dropId)
        {
            if (DropMgr.m_dropitem.ContainsKey(dropId))
                return DropMgr.m_dropitem[dropId];
            else
                return (List<DropItem>)null;
        }
    }
}
