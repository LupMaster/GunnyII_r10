using log4net;
using System;
using System.Configuration;
using System.Reflection;

namespace Bussiness
{
    public class StaticFunction
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static StaticFunction()
        {
        }

        public static bool UpdateConfig(string fileName, string name, string value)
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap()
                {
                    ExeConfigFilename = fileName
                }, ConfigurationUserLevel.None);
                configuration.AppSettings.Settings[name].Value = value;
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                if (StaticFunction.log.IsErrorEnabled)
                    StaticFunction.log.Error((object)"UpdateConfig", ex);
            }
            return false;
        }
    }
}
