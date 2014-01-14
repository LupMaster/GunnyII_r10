using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Game.Base.Config
{
    public class XMLConfigFile : ConfigElement
    {
        public XMLConfigFile()
            : base((ConfigElement)null)
        {
        }

        protected XMLConfigFile(ConfigElement parent)
            : base(parent)
        {
        }

        protected bool IsBadXMLElementName(string name)
        {
            return name != null && (name.IndexOf("\\") != -1 || name.IndexOf("/") != -1 || name.IndexOf("<") != -1 || name.IndexOf(">") != -1);
        }

        protected void SaveElement(XmlTextWriter writer, string name, ConfigElement element)
        {
            bool flag = this.IsBadXMLElementName(name);
            if (element.HasChildren)
            {
                if (name == null)
                    name = "root";
                if (flag)
                {
                    ((XmlWriter)writer).WriteStartElement("param");
                    writer.WriteAttributeString("name", name);
                }
                else
                    ((XmlWriter)writer).WriteStartElement(name);
                foreach (DictionaryEntry dictionaryEntry in element.Children)
                    this.SaveElement(writer, (string)dictionaryEntry.Key, (ConfigElement)dictionaryEntry.Value);
                writer.WriteEndElement();
            }
            else
            {
                if (name == null)
                    return;
                if (flag)
                {
                    ((XmlWriter)writer).WriteStartElement("param");
                    writer.WriteAttributeString("name", name);
                    writer.WriteString(element.GetString());
                    writer.WriteEndElement();
                }
                else
                    writer.WriteElementString(name, element.GetString());
            }
        }

        public void Save(FileInfo configFile)
        {
            if (configFile.Exists)
                configFile.Delete();
            XmlTextWriter writer = new XmlTextWriter(configFile.FullName, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            this.SaveElement(writer, (string)null, (ConfigElement)this);
            writer.WriteEndDocument();
            writer.Close();
        }

        public static XMLConfigFile ParseXMLFile(FileInfo configFile)
        {
            XMLConfigFile xmlConfigFile = new XMLConfigFile((ConfigElement)null);
            if (!configFile.Exists)
                return xmlConfigFile;
            ConfigElement parent = (ConfigElement)xmlConfigFile;
            XmlTextReader xmlTextReader = new XmlTextReader((Stream)configFile.OpenRead());
            while (xmlTextReader.Read())
            {
                if (xmlTextReader.NodeType == XmlNodeType.Element)
                {
                    if (!(xmlTextReader.Name == "root"))
                    {
                        if (xmlTextReader.Name == "param")
                        {
                            string attribute = xmlTextReader.GetAttribute("name");
                            if (attribute != null && attribute != "root")
                            {
                                ConfigElement configElement = new ConfigElement(parent);
                                parent[attribute] = configElement;
                                parent = configElement;
                            }
                        }
                        else
                        {
                            ConfigElement configElement = new ConfigElement(parent);
                            parent[xmlTextReader.Name] = configElement;
                            parent = configElement;
                        }
                    }
                }
                else if (xmlTextReader.NodeType == XmlNodeType.Text)
                    parent.Set((object)xmlTextReader.Value);
                else if (xmlTextReader.NodeType == XmlNodeType.EndElement && xmlTextReader.Name != "root")
                    parent = parent.Parent;
            }
            xmlTextReader.Close();
            return xmlConfigFile;
        }
    }
}
