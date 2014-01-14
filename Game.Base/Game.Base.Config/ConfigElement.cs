using System.Collections;

namespace Game.Base.Config
{
    public class ConfigElement
    {
        protected Hashtable m_children = new Hashtable();
        protected ConfigElement m_parent;
        protected string m_value;

        public ConfigElement this[string key]
        {
            get
            {
                lock (this.m_children)
                {
                    if (!this.m_children.Contains((object)key))
                        this.m_children.Add((object)key, (object)this.GetNewConfigElement(this));
                }
                return (ConfigElement)this.m_children[(object)key];
            }
            set
            {
                lock (this.m_children)
                    this.m_children[(object)key] = (object)value;
            }
        }

        public ConfigElement Parent
        {
            get
            {
                return this.m_parent;
            }
        }

        public bool HasChildren
        {
            get
            {
                return this.m_children.Count > 0;
            }
        }

        public Hashtable Children
        {
            get
            {
                return this.m_children;
            }
        }

        public ConfigElement(ConfigElement parent)
        {
            this.m_parent = parent;
        }

        protected virtual ConfigElement GetNewConfigElement(ConfigElement parent)
        {
            return new ConfigElement(parent);
        }

        public string GetString()
        {
            return this.m_value;
        }

        public string GetString(string defaultValue)
        {
            if (this.m_value == null)
                return defaultValue;
            else
                return this.m_value;
        }

        public int GetInt()
        {
            return int.Parse(this.m_value);
        }

        public int GetInt(int defaultValue)
        {
            if (this.m_value == null)
                return defaultValue;
            else
                return int.Parse(this.m_value);
        }

        public long GetLong()
        {
            return long.Parse(this.m_value);
        }

        public long GetLong(long defaultValue)
        {
            if (this.m_value == null)
                return defaultValue;
            else
                return long.Parse(this.m_value);
        }

        public bool GetBoolean()
        {
            return bool.Parse(this.m_value);
        }

        public bool GetBoolean(bool defaultValue)
        {
            if (this.m_value == null)
                return defaultValue;
            else
                return bool.Parse(this.m_value);
        }

        public void Set(object value)
        {
            this.m_value = value.ToString();
        }
    }
}
