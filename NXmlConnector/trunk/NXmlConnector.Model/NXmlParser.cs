using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class NXmlParser
    {
        private IDictionary<Type, Delegate> actions = new Dictionary<Type, Delegate>();

        private IDictionary<string, Type> xmlFactory = new Dictionary<string, Type>();


        public NXmlParser()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = (XmlRootAttribute)Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute), false);
                if (attribute != null)
                {
                    xmlFactory[attribute.ElementName] = type;
                }
            }
        }


        public void RegisterCallback<T>(Action<T> action)
        {
            actions[typeof(T)] = action;
        }

        public void Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return;

            var elementNameBuilder = new StringBuilder();
            var elementStart = false;
            for (int i = 0; i < xml.Length; i++)
            {
                var symbol = xml[i];
                if (elementStart) elementNameBuilder.Append(symbol);
                if (symbol == '<') elementStart = true;
                if (symbol == '>' || char.IsWhiteSpace(symbol)) break;
            }
            var elementName = elementNameBuilder.ToString().Trim(' ', '>');

            if (xmlFactory.ContainsKey(elementName))
            {
                var type = xmlFactory[elementName];
                var action = actions.ContainsKey(type) ? actions[type] : null;
                if (action != null)
                {
                    action.DynamicInvoke(NXmlDeserializer.Deserialize(type, xml));
                }
            }
        }
    }
}
