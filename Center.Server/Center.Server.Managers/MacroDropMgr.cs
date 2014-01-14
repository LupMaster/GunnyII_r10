using Bussiness;
using Center.Server;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;

namespace Center.Server.Managers
{
    public class MacroDropMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static Dictionary<int, DropInfo> m_DropInfo;
        private static string FilePath;
        private static int counter;

        static MacroDropMgr()
        {
        }

        public static bool Init()
        {
            MacroDropMgr.m_lock = new ReaderWriterLock();
            MacroDropMgr.FilePath = Directory.GetCurrentDirectory() + "\\macrodrop\\macroDrop.ini";
            return MacroDropMgr.Reload();
        }

        public static bool Reload()
        {
            try
            {
                Dictionary<int, DropInfo> dictionary1 = new Dictionary<int, DropInfo>();
                MacroDropMgr.m_DropInfo = new Dictionary<int, DropInfo>();
                Dictionary<int, DropInfo> dictionary2 = MacroDropMgr.LoadDropInfo();
                if (dictionary2 != null && dictionary2.Count > 0)
                    Interlocked.Exchange<Dictionary<int, DropInfo>>(ref MacroDropMgr.m_DropInfo, dictionary2);
                return true;
            }
            catch (Exception ex)
            {
                if (MacroDropMgr.log.IsErrorEnabled)
                    MacroDropMgr.log.Error((object)"DropInfoMgr", ex);
            }
            return false;
        }

        private static void MacroDropReset()
        {
            MacroDropMgr.m_lock.AcquireWriterLock(-1);
            try
            {
                foreach (KeyValuePair<int, DropInfo> keyValuePair in MacroDropMgr.m_DropInfo)
                {
                    int key = keyValuePair.Key;
                    DropInfo dropInfo = keyValuePair.Value;
                    if (MacroDropMgr.counter > dropInfo.Time && dropInfo.Time > 0 && MacroDropMgr.counter % dropInfo.Time == 0)
                        dropInfo.Count = dropInfo.MaxCount;
                }
            }
            catch (Exception ex)
            {
                if (!MacroDropMgr.log.IsErrorEnabled)
                    return;
                MacroDropMgr.log.Error((object)"DropInfoMgr MacroDropReset", ex);
            }
            finally
            {
                MacroDropMgr.m_lock.ReleaseWriterLock();
            }
        }

        private static void MacroDropSync()
        {
            bool flag = true;
            ServerClient[] allClients = CenterServer.Instance.GetAllClients();
            foreach (ServerClient serverClient in allClients)
            {
                if (!serverClient.NeedSyncMacroDrop)
                {
                    flag = false;
                    break;
                }
            }
            if (allClients.Length <= 0 || !flag)
                return;
            GSPacketIn pkg = new GSPacketIn((short)178);
            int count = MacroDropMgr.m_DropInfo.Count;
            pkg.WriteInt(count);
            MacroDropMgr.m_lock.AcquireReaderLock(-1);
            try
            {
                foreach (KeyValuePair<int, DropInfo> keyValuePair in MacroDropMgr.m_DropInfo)
                {
                    DropInfo dropInfo = keyValuePair.Value;
                    pkg.WriteInt(dropInfo.ID);
                    pkg.WriteInt(dropInfo.Count);
                    pkg.WriteInt(dropInfo.MaxCount);
                }
            }
            catch (Exception ex)
            {
                if (MacroDropMgr.log.IsErrorEnabled)
                    MacroDropMgr.log.Error((object)"DropInfoMgr MacroDropReset", ex);
            }
            finally
            {
                MacroDropMgr.m_lock.ReleaseReaderLock();
            }
            foreach (ServerClient serverClient in allClients)
            {
                serverClient.NeedSyncMacroDrop = false;
                serverClient.SendTCP(pkg);
            }
        }

        private static void OnTimeEvent(object source, ElapsedEventArgs e)
        {
            ++MacroDropMgr.counter;
            if (MacroDropMgr.counter % 12 == 0)
                MacroDropMgr.MacroDropReset();
            MacroDropMgr.MacroDropSync();
        }

        public static void Start()
        {
            MacroDropMgr.counter = 0;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(MacroDropMgr.OnTimeEvent);
            timer.Interval = 300000.0;
            timer.Enabled = true;
        }

        private static Dictionary<int, DropInfo> LoadDropInfo()
        {
            Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
            if (!File.Exists(MacroDropMgr.FilePath))
                return (Dictionary<int, DropInfo>)null;
            IniReader iniReader = new IniReader(MacroDropMgr.FilePath);
            for (int index = 1; iniReader.GetIniString(index.ToString(), "TemplateId") != ""; ++index)
            {
                string Section = index.ToString();
                int id = Convert.ToInt32(iniReader.GetIniString(Section, "TemplateId"));
                int time = Convert.ToInt32(iniReader.GetIniString(Section, "Time"));
                int num = Convert.ToInt32(iniReader.GetIniString(Section, "Count"));
                DropInfo dropInfo = new DropInfo(id, time, num, num);
                dictionary.Add(dropInfo.ID, dropInfo);
            }
            return dictionary;
        }

        public static void DropNotice(Dictionary<int, int> temp)
        {
            MacroDropMgr.m_lock.AcquireWriterLock(-1);
            try
            {
                foreach (KeyValuePair<int, int> keyValuePair in temp)
                {
                    if (MacroDropMgr.m_DropInfo.ContainsKey(keyValuePair.Key))
                    {
                        DropInfo dropInfo = MacroDropMgr.m_DropInfo[keyValuePair.Key];
                        if (dropInfo.Count > 0)
                            dropInfo.Count -= keyValuePair.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!MacroDropMgr.log.IsErrorEnabled)
                    return;
                MacroDropMgr.log.Error((object)"DropInfoMgr CanDrop", ex);
            }
            finally
            {
                MacroDropMgr.m_lock.ReleaseWriterLock();
            }
        }
    }
}
