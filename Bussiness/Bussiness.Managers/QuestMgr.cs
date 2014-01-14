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
    public class QuestMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();
        private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();
        private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();

        static QuestMgr()
        {
        }

        public static bool Init()
        {
            return QuestMgr.ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, QuestInfo> quests = QuestMgr.LoadQuestInfoDb();
                Dictionary<int, List<QuestConditionInfo>> dictionary1 = QuestMgr.LoadQuestCondictionDb(quests);
                Dictionary<int, List<QuestAwardInfo>> dictionary2 = QuestMgr.LoadQuestGoodDb(quests);
                if (quests.Count > 0)
                {
                    Interlocked.Exchange<Dictionary<int, QuestInfo>>(ref QuestMgr.m_questinfo, quests);
                    Interlocked.Exchange<Dictionary<int, List<QuestConditionInfo>>>(ref QuestMgr.m_questcondiction, dictionary1);
                    Interlocked.Exchange<Dictionary<int, List<QuestAwardInfo>>>(ref QuestMgr.m_questgoods, dictionary2);
                }
                return true;
            }
            catch (Exception ex)
            {
                QuestMgr.log.Error((object)"QuestMgr", ex);
            }
            return false;
        }

        public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
        {
            Dictionary<int, QuestInfo> dictionary = new Dictionary<int, QuestInfo>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                foreach (QuestInfo questInfo in produceBussiness.GetALlQuest())
                {
                    if (!dictionary.ContainsKey(questInfo.ID))
                        dictionary.Add(questInfo.ID, questInfo);
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int, QuestInfo> quests)
        {
            Dictionary<int, List<QuestConditionInfo>> dictionary = new Dictionary<int, List<QuestConditionInfo>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                QuestConditionInfo[] allQuestCondiction = produceBussiness.GetAllQuestCondiction();
                foreach (QuestInfo questInfo in quests.Values)
                {
                    QuestInfo quest = questInfo;
                    IEnumerable<QuestConditionInfo> source = Enumerable.Where<QuestConditionInfo>((IEnumerable<QuestConditionInfo>)allQuestCondiction, (Func<QuestConditionInfo, bool>)(s => s.QuestID == quest.ID));
                    dictionary.Add(quest.ID, Enumerable.ToList<QuestConditionInfo>(source));
                }
            }
            return dictionary;
        }

        public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
        {
            Dictionary<int, List<QuestAwardInfo>> dictionary = new Dictionary<int, List<QuestAwardInfo>>();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                QuestAwardInfo[] allQuestGoods = produceBussiness.GetAllQuestGoods();
                foreach (QuestInfo questInfo in quests.Values)
                {
                    QuestInfo quest = questInfo;
                    IEnumerable<QuestAwardInfo> source = Enumerable.Where<QuestAwardInfo>((IEnumerable<QuestAwardInfo>)allQuestGoods, (Func<QuestAwardInfo, bool>)(s => s.QuestID == quest.ID));
                    dictionary.Add(quest.ID, Enumerable.ToList<QuestAwardInfo>(source));
                }
            }
            return dictionary;
        }

        public static QuestInfo GetSingleQuest(int id)
        {
            if (QuestMgr.m_questinfo.ContainsKey(id))
                return QuestMgr.m_questinfo[id];
            else
                return (QuestInfo)null;
        }

        public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
        {
            if (QuestMgr.m_questgoods.ContainsKey(info.ID))
                return QuestMgr.m_questgoods[info.ID];
            else
                return (List<QuestAwardInfo>)null;
        }

        public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
        {
            if (QuestMgr.m_questcondiction.ContainsKey(info.ID))
                return QuestMgr.m_questcondiction[info.ID];
            else
                return (List<QuestConditionInfo>)null;
        }
    }
}
