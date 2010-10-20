using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public static class NXmlDeserializer
    {
        private static readonly Dictionary<string, XmlSerializer> serializers = new Dictionary<string, XmlSerializer>();


        public static object Deserialize(Type type, string xml)
        {
            using (var stringReader = new StringReader(xml))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                return GetXmlSerializer(type).Deserialize(xmlReader);
            }
        }

        private static XmlSerializer GetXmlSerializer(Type type)
        {
            lock (serializers)
            {
                var typeName = string.Format("Microsoft.Xml.Serialization.GeneratedAssembly.{0}Serializer, {1}.XmlSerializers", type.Name, new AssemblyName(type.Assembly.FullName).Name);
                if (!serializers.ContainsKey(typeName))
                {
                    var serializerType = Type.GetType(typeName, false);
                    if (serializerType != null)
                    {
                        serializers[typeName] = (XmlSerializer)Activator.CreateInstance(serializerType);
                    }
                    else
                    {
                        return new XmlSerializer(type);
                    }
                }
                return serializers[typeName];
            }
        }
    }
}
