using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ASC.Core.Tenants
{
    [Serializable]
    [XmlRoot(ElementName = "quotas")]
    public class TenantQuota : IXmlSerializable
    {
        private readonly IDictionary<string, string> props = new Dictionary<string, string>();


        public int Tenant
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Xml
        {
            get;
            set;
        }


        public TenantQuota()
        {
        }

        public TenantQuota(string xml)
        {
            Xml = xml;

            if (!string.IsNullOrEmpty(xml))
            {
                var serializer = new XmlSerializer(GetType());
                using (var reader = new StringReader(xml))
                {
                    var quota = (TenantQuota)serializer.Deserialize(reader);
                    props = quota.props;
                }
            }
        }

        public long GetQuotaInt64(string name)
        {
            string value = GetQuotaString(name);
            return string.IsNullOrEmpty(value) ? 0 : long.Parse(value, CultureInfo.InvariantCulture);
        }

        public string GetQuotaString(string name)
        {
            return props.ContainsKey(name) ? props[name] : null;
        }

        public void SetQuota(string name, long quota)
        {
            SetQuota(name, quota.ToString(CultureInfo.InvariantCulture));
        }

        public void SetQuota(string name, string quota)
        {
            props[name] = quota;
        }

        public override string ToString()
        {
            return Xml;
        }


        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (true)
            {
                if (!reader.Read()) break;
                if (reader.NodeType == XmlNodeType.Element)
                {
                    props[reader.LocalName] = reader.ReadString();
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            props.ToList().ForEach(p => writer.WriteElementString(p.Key, p.Value));
        }

        #endregion
    }
}
