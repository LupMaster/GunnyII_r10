using Game.Base.Config;
using System;
using System.IO;
using System.Net;

namespace Game.Base
{
    public class BaseServerConfiguration
    {
        protected ushort _port;
        protected IPAddress _ip;

        public ushort Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value;
            }
        }

        public IPAddress Ip
        {
            get
            {
                return this._ip;
            }
            set
            {
                this._ip = value;
            }
        }

        public BaseServerConfiguration()
        {
            this._port = (ushort)7000;
            this._ip = IPAddress.Any;
        }

        protected virtual void LoadFromConfig(ConfigElement root)
        {
            string @string = root["Server"]["IP"].GetString("any");
            this._ip = !(@string == "any") ? IPAddress.Parse(@string) : IPAddress.Any;
            this._port = (ushort)root["Server"]["Port"].GetInt((int)this._port);
        }

        public void LoadFromXMLFile(FileInfo configFile)
        {
            this.LoadFromConfig((ConfigElement)XMLConfigFile.ParseXMLFile(configFile));
        }

        protected virtual void SaveToConfig(ConfigElement root)
        {
            root["Server"]["Port"].Set((object)this._port);
            root["Server"]["IP"].Set((object)this._ip);
        }

        public void SaveToXMLFile(FileInfo configFile)
        {
            if (configFile == null)
                throw new ArgumentNullException("configFile");
            XMLConfigFile xmlConfigFile = new XMLConfigFile();
            this.SaveToConfig((ConfigElement)xmlConfigFile);
            xmlConfigFile.Save(configFile);
        }
    }
}
