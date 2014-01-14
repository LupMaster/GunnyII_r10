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
    public class TreasureAwardMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<int, TreasureAwardInfo> _treasureAward;
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        static TreasureAwardMgr()
        {
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, TreasureAwardInfo> treasureAward = new Dictionary<int, TreasureAwardInfo>();
                if (TreasureAwardMgr.Load(treasureAward))
                {
                    TreasureAwardMgr.m_lock.AcquireWriterLock(15000);
                    try
                    {
                        TreasureAwardMgr._treasureAward = treasureAward;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        TreasureAwardMgr.m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception ex)
            {
                if (TreasureAwardMgr.log.IsErrorEnabled)
                    TreasureAwardMgr.log.Error((object)"TreasureAwardMgr", ex);
            }
            return false;
        }

        public static bool Init()
        {
            bool flag;
            try
            {
                TreasureAwardMgr.m_lock = new ReaderWriterLock();
                TreasureAwardMgr._treasureAward = new Dictionary<int, TreasureAwardInfo>();
                TreasureAwardMgr.rand = new ThreadSafeRandom();
                flag = TreasureAwardMgr.Load(TreasureAwardMgr._treasureAward);
            }
            catch (Exception ex)
            {
                if (TreasureAwardMgr.log.IsErrorEnabled)
                    TreasureAwardMgr.log.Error((object)"TreasureAwardMgr", ex);
                flag = false;
            }
            return flag;
        }

        private static bool Load(Dictionary<int, TreasureAwardInfo> treasureAward)
        {
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                foreach (TreasureAwardInfo treasureAwardInfo in playerBussiness.GetAllTreasureAward())
                {
                    if (!treasureAward.ContainsKey(treasureAwardInfo.ID))
                        treasureAward.Add(treasureAwardInfo.ID, treasureAwardInfo);
                }
            }
            return true;
        }

        public static TreasureAwardInfo FindTreasureAwardInfo(int ID)
        {
            TreasureAwardMgr.m_lock.AcquireReaderLock(15000);
            try
            {
                if (TreasureAwardMgr._treasureAward.ContainsKey(ID))
                    return TreasureAwardMgr._treasureAward[ID];
            }
            catch
            {
            }
            finally
            {
                TreasureAwardMgr.m_lock.ReleaseReaderLock();
            }
            return (TreasureAwardInfo)null;
        }

        public static List<TreasureAwardInfo> GetTreasureInfos()
        {
            if (TreasureAwardMgr._treasureAward == null)
                TreasureAwardMgr.Init();
            List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
            for (int index = 1; index <= TreasureAwardMgr._treasureAward.Count; ++index)
                list.Add(TreasureAwardMgr._treasureAward[index]);
            return list;
        }

        public static List<TreasureDataInfo> CreateTreasureData(int UserID)
        {
            List<TreasureDataInfo> list = new List<TreasureDataInfo>();
            Dictionary<int, TreasureDataInfo> dictionary = new Dictionary<int, TreasureDataInfo>();
            int num = 0;
            while (list.Count < 16)
            {
                List<TreasureDataInfo> treasureData = TreasureAwardMgr.GetTreasureData();
                int index = TreasureAwardMgr.rand.Next(treasureData.Count);
                TreasureDataInfo treasureDataInfo = treasureData[index];
                treasureDataInfo.UserID = UserID;
                if (!Enumerable.Contains<int>((IEnumerable<int>)dictionary.Keys, treasureDataInfo.TemplateID))
                {
                    dictionary.Add(treasureDataInfo.TemplateID, treasureDataInfo);
                    list.Add(treasureDataInfo);
                }
                ++num;
            }
            return list;
        }

        public static List<TreasureDataInfo> GetTreasureData()
        {
            List<TreasureDataInfo> list1 = new List<TreasureDataInfo>();
            List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
            List<TreasureAwardInfo> treasureInfos = TreasureAwardMgr.GetTreasureInfos();
            int num1 = 1;
            int maxRound = ThreadSafeRandom.NextStatic(Enumerable.Max(Enumerable.Select<TreasureAwardInfo, int>((IEnumerable<TreasureAwardInfo>)treasureInfos, (Func<TreasureAwardInfo, int>)(s => s.Random))));
            List<TreasureAwardInfo> list3 = Enumerable.ToList<TreasureAwardInfo>(Enumerable.Where<TreasureAwardInfo>((IEnumerable<TreasureAwardInfo>)treasureInfos, (Func<TreasureAwardInfo, bool>)(s => s.Random >= maxRound)));
            int num2 = Enumerable.Count<TreasureAwardInfo>((IEnumerable<TreasureAwardInfo>)list3);
            if (num2 > 0)
            {
                int count = num1 > num2 ? num2 : num1;
                foreach (int index in TreasureAwardMgr.GetRandomUnrepeatArray(0, num2 - 1, count))
                {
                    TreasureAwardInfo treasureAwardInfo = list3[index];
                    list2.Add(treasureAwardInfo);
                }
            }
            foreach (TreasureAwardInfo treasureAwardInfo in list2)
                list1.Add(new TreasureDataInfo()
                {
                    ID = 0,
                    UserID = 0,
                    TemplateID = treasureAwardInfo.TemplateID,
                    Count = treasureAwardInfo.Count,
                    ValidDate = treasureAwardInfo.Validate,
                    pos = -1,
                    BeginDate = DateTime.Now,
                    IsExit = true
                });
            return list1;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int[] numArray = new int[count];
            for (int index1 = 0; index1 < count; ++index1)
            {
                int num1 = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
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
