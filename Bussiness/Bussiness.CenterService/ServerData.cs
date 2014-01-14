using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Bussiness.CenterService
{
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Name = "ServerData", Namespace = "http://schemas.datacontract.org/2004/07/Center.Server")]
    [DebuggerStepThrough]
    [Serializable]
    public class ServerData : IExtensibleDataObject, INotifyPropertyChanged
    {
        [NonSerialized]
        private ExtensionDataObject extensionDataField;
        [OptionalField]
        private int IdField;
        [OptionalField]
        private string IpField;
        [OptionalField]
        private int LowestLevelField;
        [OptionalField]
        private int MustLevelField;
        [OptionalField]
        private string NameField;
        [OptionalField]
        private int OnlineField;
        [OptionalField]
        private int PortField;
        [OptionalField]
        private int StateField;

        [Browsable(false)]
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [DataMember]
        public int Id
        {
            get
            {
                return this.IdField;
            }
            set
            {
                if (this.IdField.Equals(value))
                    return;
                this.IdField = value;
                this.RaisePropertyChanged("Id");
            }
        }

        [DataMember]
        public string Ip
        {
            get
            {
                return this.IpField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.IpField, (object)value))
                    return;
                this.IpField = value;
                this.RaisePropertyChanged("Ip");
            }
        }

        [DataMember]
        public int LowestLevel
        {
            get
            {
                return this.LowestLevelField;
            }
            set
            {
                if (this.LowestLevelField.Equals(value))
                    return;
                this.LowestLevelField = value;
                this.RaisePropertyChanged("LowestLevel");
            }
        }

        [DataMember]
        public int MustLevel
        {
            get
            {
                return this.MustLevelField;
            }
            set
            {
                if (this.MustLevelField.Equals(value))
                    return;
                this.MustLevelField = value;
                this.RaisePropertyChanged("MustLevel");
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                if (object.ReferenceEquals((object)this.NameField, (object)value))
                    return;
                this.NameField = value;
                this.RaisePropertyChanged("Name");
            }
        }

        [DataMember]
        public int Online
        {
            get
            {
                return this.OnlineField;
            }
            set
            {
                if (this.OnlineField.Equals(value))
                    return;
                this.OnlineField = value;
                this.RaisePropertyChanged("Online");
            }
        }

        [DataMember]
        public int Port
        {
            get
            {
                return this.PortField;
            }
            set
            {
                if (this.PortField.Equals(value))
                    return;
                this.PortField = value;
                this.RaisePropertyChanged("Port");
            }
        }

        [DataMember]
        public int State
        {
            get
            {
                return this.StateField;
            }
            set
            {
                if (this.StateField.Equals(value))
                    return;
                this.StateField = value;
                this.RaisePropertyChanged("State");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler((object)this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
