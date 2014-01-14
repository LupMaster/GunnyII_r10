using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Bussiness.Managers
{
    public class AchievementMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, AchievementInfo> m_achievement = new Dictionary<int, AchievementInfo>();
        private static Dictionary<int, List<AchievementConditionInfo>> m_achievementCondition = new Dictionary<int, List<AchievementConditionInfo>>();
        private static Dictionary<int, List<AchievementRewardInfo>> m_achievementReward = new Dictionary<int, List<AchievementRewardInfo>>();
        private static Hashtable m_distinctCondition = new Hashtable();
        private static Dictionary<int, List<ItemRecordTypeInfo>> m_itemRecordType = new Dictionary<int, List<ItemRecordTypeInfo>>();
        private static Hashtable m_ItemRecordTypeInfo = new Hashtable();
        private static Dictionary<int, List<int>> m_recordLimit = new Dictionary<int, List<int>>();

        public static Dictionary<int, AchievementInfo> Achievement
        {
            get
            {
                return AchievementMgr.m_achievement;
            }
        }

        public static Hashtable ItemRecordType
        {
            get
            {
                return AchievementMgr.m_ItemRecordTypeInfo;
            }
        }

        static AchievementMgr()
        {
        }

        public static List<AchievementConditionInfo> GetAchievementCondition(AchievementInfo info)
        {
            if (AchievementMgr.m_achievementCondition.ContainsKey(info.ID))
                return AchievementMgr.m_achievementCondition[info.ID];
            else
                return (List<AchievementConditionInfo>)null;
        }

        public static List<AchievementRewardInfo> GetAchievementReward(AchievementInfo info)
        {
            if (AchievementMgr.m_achievementReward.ContainsKey(info.ID))
                return AchievementMgr.m_achievementReward[info.ID];
            else
                return (List<AchievementRewardInfo>)null;
        }

        public static int GetNextLimit(int recordType, int recordValue)
        {
            if (!AchievementMgr.m_recordLimit.ContainsKey(recordType))
                return int.MaxValue;
            foreach (int num in AchievementMgr.m_recordLimit[recordType])
            {
                if (num > recordValue)
                    return num;
            }
            return int.MaxValue;
        }

        public static AchievementInfo GetSingleAchievement(int id)
        {
            if (AchievementMgr.m_achievement.ContainsKey(id))
                return AchievementMgr.m_achievement[id];
            else
                return (AchievementInfo)null;
        }

        public static bool Init()
        {
            return AchievementMgr.Reload();
        }

        public static Dictionary<int, List<AchievementConditionInfo>> LoadAchievementConditionInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
        {
            Dictionary<int, List<AchievementConditionInfo>> dictionary = new Dictionary<int, List<AchievementConditionInfo>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                AchievementConditionInfo[] achievementCondition = produceBussiness.GetALlAchievementCondition();
                using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achievementInfos.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AchievementInfo achievementInfo = enumerator.Current;
                        IEnumerable<AchievementConditionInfo> source = Enumerable.Where<AchievementConditionInfo>((IEnumerable<AchievementConditionInfo>)achievementCondition, (Func<AchievementConditionInfo, bool>)(s => s.AchievementID == achievementInfo.ID));
                        dictionary.Add(achievementInfo.ID, Enumerable.ToList<AchievementConditionInfo>(source));
                        if (source != null)
                        {
                            foreach (AchievementConditionInfo achievementConditionInfo in source)
                            {
                                if (!AchievementMgr.m_distinctCondition.Contains((object)achievementConditionInfo.CondictionType))
                                    AchievementMgr.m_distinctCondition.Add((object)achievementConditionInfo.CondictionType, (object)achievementConditionInfo.CondictionType);
                            }
                        }
                    }
                }
                foreach (AchievementConditionInfo achievementConditionInfo in achievementCondition)
                {
                    int condictionType = achievementConditionInfo.CondictionType;
                    int condictionPara2 = achievementConditionInfo.Condiction_Para2;
                    if (!AchievementMgr.m_recordLimit.ContainsKey(condictionType))
                        AchievementMgr.m_recordLimit.Add(condictionType, new List<int>());
                    if (!AchievementMgr.m_recordLimit[condictionType].Contains(condictionPara2))
                        AchievementMgr.m_recordLimit[condictionType].Add(condictionPara2);
                }
                foreach (int index in AchievementMgr.m_recordLimit.Keys)
                    AchievementMgr.m_recordLimit[index].Sort();
            }
            return dictionary;
        }

        public static Dictionary<int, AchievementInfo> LoadAchievementInfoDB()
        {
            Dictionary<int, AchievementInfo> dictionary = new Dictionary<int, AchievementInfo>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (AchievementInfo achievementInfo in produceBussiness.GetALlAchievement())
                {
                    if (!dictionary.ContainsKey(achievementInfo.ID))
                        dictionary.Add(achievementInfo.ID, achievementInfo);
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<AchievementRewardInfo>> LoadAchievementRewardInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
        {
            Dictionary<int, List<AchievementRewardInfo>> dictionary = new Dictionary<int, List<AchievementRewardInfo>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                AchievementRewardInfo[] achievementReward = produceBussiness.GetALlAchievementReward();
                using (Dictionary<int, AchievementInfo>.ValueCollection.Enumerator enumerator = achievementInfos.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        AchievementInfo achievementInfo = enumerator.Current;
                        IEnumerable<AchievementRewardInfo> source = Enumerable.Where<AchievementRewardInfo>((IEnumerable<AchievementRewardInfo>)achievementReward, (Func<AchievementRewardInfo, bool>)(s => s.AchievementID == achievementInfo.ID));
                        dictionary.Add(achievementInfo.ID, Enumerable.ToList<AchievementRewardInfo>(source));
                    }
                }
            }
            return dictionary;
        }

        public static void LoadItemRecordTypeInfoDB()
        {
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (ItemRecordTypeInfo itemRecordTypeInfo in produceBussiness.GetAllItemRecordType())
                {
                    if (!AchievementMgr.m_ItemRecordTypeInfo.Contains((object)itemRecordTypeInfo.RecordID))
                        AchievementMgr.m_ItemRecordTypeInfo.Add((object)itemRecordTypeInfo.RecordID, (object)itemRecordTypeInfo.Name);
                }
            }
        }

        public static bool Reload()
        {
            try
            {
                AchievementMgr.LoadItemRecordTypeInfoDB();
                Dictionary<int, AchievementInfo> achievementInfos = AchievementMgr.LoadAchievementInfoDB();
                Dictionary<int, List<AchievementConditionInfo>> dictionary1 = AchievementMgr.LoadAchievementConditionInfoDB(achievementInfos);
                Dictionary<int, List<AchievementRewardInfo>> dictionary2 = AchievementMgr.LoadAchievementRewardInfoDB(achievementInfos);
                if (achievementInfos.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, AchievementInfo>>(ref AchievementMgr.m_achievement, achievementInfos);
                    Interlocked.Exchange<Dictionary<int, List<AchievementConditionInfo>>>(ref AchievementMgr.m_achievementCondition, dictionary1);
                    Interlocked.Exchange<Dictionary<int, List<AchievementRewardInfo>>>(ref AchievementMgr.m_achievementReward, dictionary2);
                }
                return true;
            }
            catch (Exception ex)
            {
                AchievementMgr.log.Error((object)"AchievementMgr", ex);
            }
            return false;
        }
    }
}
