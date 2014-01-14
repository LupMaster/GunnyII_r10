using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class ActiveMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Dictionary<int, ActiveAwardInfo> m_ActiveAwardInfo = new Dictionary<int, ActiveAwardInfo>();
        public static Dictionary<int, List<ActiveConditionInfo>> m_ActiveConditionInfo = new Dictionary<int, List<ActiveConditionInfo>>();

        static ActiveMgr()
        {
        }

        public static List<ActiveAwardInfo> GetAwardInfo(DateTime lastDate, int playerGrade)
        {
            string str1 = (string)null;
            int days = (DateTime.Now - lastDate).Days;
            if (DateTime.Now.DayOfYear > lastDate.DayOfYear)
                ++days;
            List<ActiveAwardInfo> list1 = new List<ActiveAwardInfo>();
            foreach (List<ActiveConditionInfo> list2 in ActiveMgr.m_ActiveConditionInfo.Values)
            {
                foreach (ActiveConditionInfo info in list2)
                {
                    if (ActiveMgr.IsValid(info) && ActiveMgr.IsInGrade(info.LimitGrade, playerGrade) && info.Condition <= days)
                    {
                        str1 = info.AwardId;
                        int activeId = info.ActiveID;
                    }
                }
            }
            if (!string.IsNullOrEmpty(str1))
            {
                string str2 = str1;
                char[] chArray = new char[1]
        {
          ','
        };
                foreach (string str3 in str2.Split(chArray))
                {
                    if (!string.IsNullOrEmpty(str3) && ActiveMgr.m_ActiveAwardInfo.ContainsKey(Convert.ToInt32(str3)))
                        list1.Add(ActiveMgr.m_ActiveAwardInfo[Convert.ToInt32(str3)]);
                }
            }
            return list1;
        }

        public static bool Init()
        {
            return ActiveMgr.ReLoad();
        }

        private static bool IsInGrade(string limitGrade, int playerGrade)
        {
            bool flag = false;
            int num1 = 0;
            int num2 = 0;
            if (limitGrade != null)
            {
                string[] strArray = limitGrade.Split(new char[1]
        {
          '-'
        });
                if (strArray.Length == 2)
                {
                    num1 = Convert.ToInt32(strArray[0]);
                    num2 = Convert.ToInt32(strArray[1]);
                }
                if (num1 <= playerGrade && num2 >= playerGrade)
                    flag = true;
            }
            return flag;
        }

        public static bool IsValid(ActiveConditionInfo info)
        {
            DateTime startTime = info.StartTime;
            DateTime endTime = info.EndTime;
            DateTime dateTime1 = info.StartTime;
            long ticks1 = dateTime1.Ticks;
            dateTime1 = DateTime.Now;
            long ticks2 = dateTime1.Ticks;
            int num;
            if (ticks1 <= ticks2)
            {
                DateTime dateTime2 = info.EndTime;
                long ticks3 = dateTime2.Ticks;
                dateTime2 = DateTime.Now;
                long ticks4 = dateTime2.Ticks;
                num = ticks3 >= ticks4 ? 1 : 0;
            }
            else
                num = 0;
            return num != 0;
        }

        public static Dictionary<int, ActiveAwardInfo> LoadActiveAwardDb(Dictionary<int, List<ActiveConditionInfo>> conditions)
        {
            Dictionary<int, ActiveAwardInfo> dictionary = new Dictionary<int, ActiveAwardInfo>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                ActiveAwardInfo[] allActiveAwardInfo = produceBussiness.GetAllActiveAwardInfo();
                foreach (int num in conditions.Keys)
                {
                    foreach (ActiveAwardInfo activeAwardInfo in allActiveAwardInfo)
                    {
                        if (num == activeAwardInfo.ActiveID && !dictionary.ContainsKey(activeAwardInfo.ID))
                            dictionary.Add(activeAwardInfo.ID, activeAwardInfo);
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<ActiveConditionInfo>> LoadActiveConditionDb()
        {
            Dictionary<int, List<ActiveConditionInfo>> dictionary = new Dictionary<int, List<ActiveConditionInfo>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (ActiveConditionInfo activeConditionInfo in produceBussiness.GetAllActiveConditionInfo())
                {
                    List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
                    if (!dictionary.ContainsKey(activeConditionInfo.ActiveID))
                    {
                        list.Add(activeConditionInfo);
                        dictionary.Add(activeConditionInfo.ActiveID, list);
                    }
                    else
                        dictionary[activeConditionInfo.ActiveID].Add(activeConditionInfo);
                }
            }
            return dictionary;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, List<ActiveConditionInfo>> conditions = ActiveMgr.LoadActiveConditionDb();
                Dictionary<int, ActiveAwardInfo> dictionary = ActiveMgr.LoadActiveAwardDb(conditions);
                if (conditions.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, List<ActiveConditionInfo>>>(ref ActiveMgr.m_ActiveConditionInfo, conditions);
                    Interlocked.Exchange<Dictionary<int, ActiveAwardInfo>>(ref ActiveMgr.m_ActiveAwardInfo, dictionary);
                }
                return true;
            }
            catch (Exception ex)
            {
                ActiveMgr.log.Error((object)"QuestMgr", ex);
            }
            return false;
        }
    }
}
