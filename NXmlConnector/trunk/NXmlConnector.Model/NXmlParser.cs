using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class NXmlParser
    {
        private Dictionary<string, KeyValuePair<Type, Delegate>> actions = new Dictionary<string, KeyValuePair<Type, Delegate>>();


        public void RegisterCallback<T>(Action<T> action)
        {
            var type = typeof(T);
            var attribute = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), false);
            if (attribute == null) throw new ArgumentException(string.Format("Невозможно зарегистрировать функцию обратного вызова для типа {0}, т.к. тип не имеет атрибута XmlRootAttribute.", type));

            actions[attribute.ElementName] = new KeyValuePair<Type, Delegate>(type, action);
        }

        public void Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return;

            var elementName = string.Empty;
            var elementStart = false;
            foreach (var symbol in xml)
            {
                if (symbol == '<') elementStart = true;
                else if (symbol == '>' || char.IsWhiteSpace(symbol)) break;
                else if (elementStart) elementName += symbol;
            }

            if (actions.ContainsKey(elementName))
            {
                actions[elementName].Value.DynamicInvoke(NXmlDeserializer.Deserialize(actions[elementName].Key, xml));
            }
        }
    }
}
