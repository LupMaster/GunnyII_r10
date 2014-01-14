using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Bussiness
{
    public class LanguageMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Hashtable LangsSentences = new Hashtable();
        public static List<string> NotceList = new List<string>();

        private static string LanguageFile
        {
            get
            {
                return ConfigurationManager.AppSettings["LanguagePath"];
            }
        }

        private static string SystemNoticeFile
        {
            get
            {
                return ConfigurationManager.AppSettings["SystemNoticePath"];
            }
        }

        static LanguageMgr()
        {
        }

        public static bool Setup(string path)
        {
            return LanguageMgr.Reload(path) && LanguageMgr.LoadNotice("");
        }

        public static bool LoadNotice(string path)
        {
            string str1 = path + LanguageMgr.SystemNoticeFile;
            if (!File.Exists(str1))
            {
                LanguageMgr.log.Error((object)("SystemNotice file : " + str1 + " not found !"));
            }
            else
            {
                try
                {
                    foreach (XElement xelement in XDocument.Load(str1).Root.Nodes())
                    {
                        try
                        {
                            int.Parse(xelement.Attribute((XName)"id").Value);
                            string str2 = xelement.Attribute((XName)"notice").Value;
                            LanguageMgr.NotceList.Add(str2);
                        }
                        catch (Exception ex)
                        {
                            LanguageMgr.log.Error((object)"BattleMgr setup error:", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LanguageMgr.log.Error((object)"BattleMgr setup error:", ex);
                }
            }
            LanguageMgr.log.InfoFormat("Total {0} syterm notice loaded.", (object)LanguageMgr.NotceList.Count);
            return true;
        }

        public static bool Reload(string path)
        {
            try
            {
                Hashtable hashtable = LanguageMgr.LoadLanguage(path);
                if (hashtable.Count > 0)
                {
                    Interlocked.Exchange<Hashtable>(ref LanguageMgr.LangsSentences, hashtable);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LanguageMgr.log.Error((object)"Load language file error:", ex);
            }
            return false;
        }

        private static Hashtable LoadLanguage(string path)
        {
            Hashtable hashtable = new Hashtable();
            string path1 = path + LanguageMgr.LanguageFile;
            if (!File.Exists(path1))
            {
                LanguageMgr.log.Error((object)("Language file : " + path1 + " not found !"));
            }
            else
            {
                foreach (string str in (IEnumerable)new ArrayList((ICollection)File.ReadAllLines(path1, Encoding.UTF8)))
                {
                    if (!str.StartsWith("#") && str.IndexOf(':') != -1)
                    {
                        string[] strArray = new string[2]
            {
              str.Substring(0, str.IndexOf(':')),
              str.Substring(str.IndexOf(':') + 1)
            };
                        strArray[1] = strArray[1].Replace("\t", "");
                        hashtable[(object)strArray[0]] = (object)strArray[1];
                    }
                }
            }
            return hashtable;
        }

        public static string GetTranslation(string translateId, params object[] args)
        {
            if (!LanguageMgr.LangsSentences.ContainsKey((object)translateId))
                return translateId;
            string format = (string)LanguageMgr.LangsSentences[(object)translateId];
            try
            {
                format = string.Format(format, args);
            }
            catch (Exception ex)
            {
                LanguageMgr.log.Error((object)("Parameters number error, ID: " + (object)translateId + " (Arg count=" + (string)(object)args.Length + ")"), ex);
            }
            return format ?? translateId;
        }
    }
}
