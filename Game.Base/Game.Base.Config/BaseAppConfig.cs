using log4net;
using System;
using System.Configuration;
using System.Reflection;

namespace Game.Base.Config
{
    public abstract class BaseAppConfig
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static BaseAppConfig()
        {
        }

        protected virtual void Load(Type type)
        {
            ConfigurationManager.RefreshSection("appSettings");
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), false);
                if (customAttributes.Length != 0)
                {
                    ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
                    fieldInfo.SetValue((object)this, this.LoadConfigProperty(attrib));
                }
            }
        }

        private object LoadConfigProperty(ConfigPropertyAttribute attrib)
        {
            string key = attrib.Key;
            string str = ConfigurationManager.AppSettings[key];
            if (str == null)
            {
                str = attrib.DefaultValue.ToString();
                BaseAppConfig.log.Warn((object)("Loading " + key + " value is null,using default vaule:" + str));
            }
            else
                BaseAppConfig.log.Debug((object)("Loading " + key + " Value is " + str));
            object obj;
            try
            {
                obj = Convert.ChangeType((object)str, attrib.DefaultValue.GetType());
            }
            catch (Exception ex)
            {
                BaseAppConfig.log.Error((object)"Exception in ServerProperties Load: ", ex);
                obj = (object)null;
            }
            return obj;
        }
    }
}
