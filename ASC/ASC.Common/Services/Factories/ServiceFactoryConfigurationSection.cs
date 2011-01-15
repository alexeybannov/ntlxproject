#region usings

using System.Configuration;

#endregion

namespace ASC.Common.Services.Factories
{
    public class ServiceFactoryConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("factories", IsDefaultCollection = false)]
        public FactoriesCollection Factories
        {
            get { return (FactoriesCollection) base["factories"]; }
        }
    }

    public class FactoriesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FactoryElement) element).Service;
        }

        public void Add(FactoryElement element)
        {
            BaseAdd(element);
        }
    }

    public class FactoryElement : ConfigurationElement
    {
        [ConfigurationProperty("service", IsKey = true)]
        public string Service
        {
            get { return (string) this["service"]; }
            set { this["service"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string FactoryType
        {
            get { return (string) this["type"]; }
            set { this["type"] = value; }
        }
    }
}