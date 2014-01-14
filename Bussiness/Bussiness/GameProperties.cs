using Game.Base.Config;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bussiness
{
    public abstract class GameProperties
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [ConfigProperty("FreeMoney", "µ±Ç°ÓÎÏ·°æ±\x00BE", "0")]
        public static readonly string FreeMoney;
        [ConfigProperty("FreeExp", "µ±Ç°ÓÎÏ·°æ±\x00BE", "11901")]
        public static readonly string FreeExp;
        [ConfigProperty("BigExp", "µ±Ç°ÓÎÏ·°æ±\x00BE", "11906")]
        public static readonly string BigExp;
        [ConfigProperty("PetExp", "µ±Ç°ÓÎÏ·°æ±\x00BE", "334103")]
        public static readonly string PetExp;
        [ConfigProperty("Edition", "µ±Ç°ÓÎÏ·°æ±\x00BE", "11000")]
        public static readonly string EDITION;
        [ConfigProperty("MustComposeGold", "ºÏ\x00B3ÉÏûºÄ\x00BDð±Ò\x00BCÛ¸ñ", 1000)]
        public static readonly int PRICE_COMPOSE_GOLD;
        [ConfigProperty("MustFusionGold", "ÈÛÁ¶ÏûºÄ\x00BDð±Ò\x00BCÛ¸ñ", 1000)]
        public static readonly int PRICE_FUSION_GOLD;
        [ConfigProperty("MustStrengthenGold", "Ç¿»¯\x00BDð±ÒÏûºÄ\x00BCÛ¸ñ", 1000)]
        public static readonly int PRICE_STRENGHTN_GOLD;
        [ConfigProperty("CheckRewardItem", "ÑéÖ¤Âë\x00BD±ÀøÎïÆ·", 11001)]
        public static readonly int CHECK_REWARD_ITEM;
        [ConfigProperty("CheckCount", "×î´óÑéÖ¤ÂëÊ§°Ü´ÎÊý", 2)]
        public static readonly int CHECK_MAX_FAILED_COUNT;
        [ConfigProperty("HymenealMoney", "Çó»éµÄ\x00BCÛ¸ñ", 300)]
        public static readonly int PRICE_PROPOSE;
        [ConfigProperty("DivorcedMoney", "Àë»éµÄ\x00BCÛ¸ñ", 1499)]
        public static readonly int PRICE_DIVORCED;
        [ConfigProperty("DivorcedDiscountMoney", "Àë»éµÄ\x00BCÛ¸ñ", 999)]
        public static readonly int PRICE_DIVORCED_DISCOUNT;
        [ConfigProperty("MarryRoomCreateMoney", "\x00BDá»é·¿\x00BCäµÄ\x00BCÛ¸ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö¸ô", "2000,2700,3400")]
        public static readonly string PRICE_MARRY_ROOM;
        [ConfigProperty("BoxAppearCondition", "Ïä×ÓÎïÆ·ÌáÊ\x00BEµÄµÈ\x00BC¶", 4)]
        public static readonly int BOX_APPEAR_CONDITION;
        [ConfigProperty("DisableCommands", "\x00BDûÖ\x00B9Ê\x00B9ÓÃµÄÃüÁî", "")]
        public static readonly string DISABLED_COMMANDS;
        [ConfigProperty("AssState", "·À\x00B3ÁÃÔÏµÍ\x00B3µÄ¿ª\x00B9Ø,True´ò¿ª,False\x00B9Ø±Õ", false)]
        public static bool ASS_STATE;
        [ConfigProperty("DailyAwardState", "Ã¿ÈÕ\x00BD±Àø¿ª\x00B9Ø,True´ò¿ª,False\x00B9Ø±Õ", true)]
        public static bool DAILY_AWARD_STATE;
        [ConfigProperty("Cess", "\x00BD»Ò×¿ÛË°", 0.1)]
        public static readonly double Cess;
        [ConfigProperty("BeginAuction", "ÅÄÂòÊ±ÆðÊ\x00BCËæ»úÊ±\x00BCä", 20)]
        public static int BeginAuction;
        [ConfigProperty("EndAuction", "ÅÄÂòÊ±\x00BDáÊøËæ»úÊ±\x00BCä", 40)]
        public static int EndAuction;
        [ConfigProperty("HotSpringExp", "Kinh nghiệm Spa", "1|2")]
        public static readonly string HotSpringExp;
        [ConfigProperty("ConsortiaStrengthenEx", "Kinh nghiệm", "1|2")]
        public static readonly string ConsortiaStrengthenEx;
        [ConfigProperty("RuneLevelUpExp", "Kinh nghiệm châu báu", "1|2")]
        public static readonly string RuneLevelUpExp;
        [ConfigProperty("RunePackageID", "RunePackageID", "1|2")]
        public static readonly string RunePackageID;
        [ConfigProperty("OpenRunePackageMoney", "OpenRunePackageMoney", "1|2")]
        public static readonly string OpenRunePackageMoney;
        [ConfigProperty("OpenRunePackageRange", "OpenRunePackageRange", "1|2")]
        public static readonly string OpenRunePackageRange;
        [ConfigProperty("VIPExpForEachLv", "VIPExpForEachLv", "1|2")]
        public static readonly string VIPExpForEachLv;
        [ConfigProperty("HoleLevelUpExpList", "HoleLevelUpExpList", "1|2")]
        public static readonly string HoleLevelUpExpList;
        [ConfigProperty("VIPStrengthenEx", "VIPStrengthenEx", "1|2")]
        public static readonly string VIPStrengthenEx;

        static GameProperties()
        {
        }

        private static void Load(Type type)
        {
            using (ServiceBussiness sb = new ServiceBussiness())
            {
                foreach (FieldInfo fieldInfo in type.GetFields())
                {
                    if (fieldInfo.IsStatic)
                    {
                        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                        if (customAttributes.Length != 0)
                        {
                            ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
                            fieldInfo.SetValue((object)null, GameProperties.LoadProperty(attrib, sb));
                        }
                    }
                }
            }
        }

        private static void Save(Type type)
        {
            using (ServiceBussiness sb = new ServiceBussiness())
            {
                foreach (FieldInfo fieldInfo in type.GetFields())
                {
                    if (fieldInfo.IsStatic)
                    {
                        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                        if (customAttributes.Length != 0)
                            GameProperties.SaveProperty((ConfigPropertyAttribute)customAttributes[0], sb, fieldInfo.GetValue((object)null));
                    }
                }
            }
        }

        private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
        {
            string key = attrib.Key;
            ServerProperty serverProperty = sb.GetServerPropertyByKey(key);
            if (serverProperty == null)
            {
                serverProperty = new ServerProperty();
                serverProperty.Key = key;
                serverProperty.Value = attrib.DefaultValue.ToString();
                GameProperties.log.Error((object)("Cannot find server property " + key + ",keep it default value!"));
            }
            object obj;
            try
            {
                obj = Convert.ChangeType((object)serverProperty.Value, attrib.DefaultValue.GetType());
            }
            catch (Exception ex)
            {
                GameProperties.log.Error((object)"Exception in GameProperties Load: ", ex);
                obj = (object)null;
            }
            return obj;
        }

        private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
        {
            try
            {
                sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
            }
            catch (Exception ex)
            {
                GameProperties.log.Error((object)"Exception in GameProperties Save: ", ex);
            }
        }

        public static void Refresh()
        {
            GameProperties.log.Info((object)"Refreshing game properties!");
            GameProperties.Load(typeof(GameProperties));
        }

        public static List<int> getProp(string prop)
        {
            List<int> list = new List<int>();
            string str1 = prop;
            char[] chArray = new char[1]
      {
        '|'
      };
            foreach (string str2 in str1.Split(chArray))
                list.Add(Convert.ToInt32(str2));
            return list;
        }

        public static List<int> VIPExp()
        {
            return GameProperties.getProp(GameProperties.VIPExpForEachLv);
        }

        public static List<int> RuneExp()
        {
            return GameProperties.getProp(GameProperties.RuneLevelUpExp);
        }

        public static int ConsortiaStrengExp(int Lv)
        {
            return GameProperties.getProp(GameProperties.ConsortiaStrengthenEx)[Lv];
        }

        public static int VIPStrengthenExp(int vipLv)
        {
            return GameProperties.getProp(GameProperties.VIPStrengthenEx)[vipLv];
        }

        public static int HoleLevelUpExp(int lv)
        {
            return GameProperties.getProp(GameProperties.HoleLevelUpExpList)[lv];
        }

        public static void Save()
        {
            GameProperties.log.Info((object)"Saving game properties into db!");
            GameProperties.Save(typeof(GameProperties));
        }
    }
}
