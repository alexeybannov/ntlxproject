#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

#endregion

namespace ASC.Notify.Model
{
    public sealed class XmlActionProvider : IActionProvider
    {
        private ConstActionProvider _ConstProvider;

        public XmlActionProvider(string xmlPath)
        {
            if (xmlPath == null) throw new ArgumentNullException("xmlPath");
            LoadXml(XmlReader.Create(xmlPath));
        }

        public XmlActionProvider(Stream xmlStream)
        {
            if (xmlStream == null) throw new ArgumentNullException("xmlStream");
            LoadXml(XmlReader.Create(xmlStream));
        }

        public XmlActionProvider(Assembly assembly, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            LoadXml(XmlReader.Create(stream));
        }

        public XmlActionProvider(XmlReader xmlReader)
        {
            if (xmlReader == null) throw new ArgumentNullException("xmlReader");
            LoadXml(xmlReader);
        }

        #region IActionProvider

        public INotifyAction GetAction(string id)
        {
            return _ConstProvider.GetAction(id);
        }

        public INotifyAction[] GetActions()
        {
            return _ConstProvider.GetActions();
        }

        #endregion

        private void LoadXml(XmlReader xmlReader)
        {
            var result = new List<INotifyAction>();
            XPathNavigator nav = new XPathDocument(xmlReader).CreateNavigator();
            var manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("act", "urn:asc.notify.action.xsd");
            XPathNodeIterator nodes = nav.Select("act:catalog/action", manager);
            while (nodes.MoveNext())
            {
                result.Add(
                    new NotifyAction(
                        nodes.Current.GetAttribute("id", ""),
                        nodes.Current.GetAttribute("name", "")));
            }
            _ConstProvider = new ConstActionProvider(result.ToArray());
        }
    }
}