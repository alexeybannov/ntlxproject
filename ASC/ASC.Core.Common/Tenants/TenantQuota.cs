#region usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace ASC.Core.Tenants
{
    [Serializable]
    [XmlRoot(ElementName = "quotas")]
    public class TenantQuota : IXmlSerializable
    {
        private IDictionary<string, string> props = new Dictionary<string, string>();

        public TenantQuota()
        {
        }

        public TenantQuota(string quotasString)
        {
            QuotasString = quotasString;
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
            return QuotasString;
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

        internal string QuotasString
        {
            get
            {
                var serializer = new XmlSerializer(typeof (TenantQuota));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, this, new XmlSerializerNamespaces(new[] {XmlQualifiedName.Empty}));
                    return writer.ToString();
                }
            }
            set
            {
                var serializer = new XmlSerializer(typeof (TenantQuota));
                using (var reader = new StringReader(value))
                {
                    var quota = (TenantQuota) serializer.Deserialize(reader);
                    props = quota.props;
                }
            }
        }
    }
}