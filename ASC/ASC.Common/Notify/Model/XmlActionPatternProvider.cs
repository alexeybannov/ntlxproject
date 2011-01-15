#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using ASC.Common.Utils;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Model
{
    #region usings

    using Accord = KeyValuePair<string, IPattern>;
    using TernarAccord = KeyValuePair<INotifyAction, KeyValuePair<string, IPattern>>;

    #endregion

    public sealed class XmlActionPatternProvider
        : IActionPatternProvider
    {
        private readonly IActionProvider _ActionProvider;
        private readonly IPatternProvider _PatternProvider;
        private ConstActionPatternProvider _ConstProvider;

        public XmlActionPatternProvider(string xmlPath, IActionProvider actionProvider, IPatternProvider patternProvider)
        {
            if (xmlPath == null) throw new ArgumentNullException("xmlPath");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");
            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(XmlReader.Create(xmlPath));
        }

        public XmlActionPatternProvider(Stream xmlStream, IActionProvider actionProvider,
                                        IPatternProvider patternProvider)
        {
            if (xmlStream == null) throw new ArgumentNullException("xmlStream");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");
            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(XmlReader.Create(xmlStream));
        }

        public XmlActionPatternProvider(Assembly assembly, string resourceName, IActionProvider actionProvider,
                                        IPatternProvider patternProvider)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");
            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            LoadXml(XmlReader.Create(stream));
        }

        public XmlActionPatternProvider(XmlReader xmlReader, IActionProvider actionProvider,
                                        IPatternProvider patternProvider)
        {
            if (xmlReader == null) throw new ArgumentNullException("xmlReader");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            if (patternProvider == null) throw new ArgumentNullException("patternProvider");
            _ActionProvider = actionProvider;
            _PatternProvider = patternProvider;
            LoadXml(xmlReader);
        }

        #region IActionPatternProvider

        public IPattern GetPattern(INotifyAction action, string senderName)
        {
            return _ConstProvider.GetPattern(action, senderName);
        }

        public IPattern GetPattern(INotifyAction action)
        {
            return _ConstProvider.GetPattern(action);
        }

        public GetPatternCallback GetPatternMethod { get; set; }

        #endregion

        private void LoadXml(XmlReader xmlReader)
        {
            var result = new List<TernarAccord>();
            XPathNavigator nav = new XPathDocument(xmlReader).CreateNavigator();
            var manager = new XmlNamespaceManager(nav.NameTable);
            manager.AddNamespace("act", "urn:asc.notify.action_pattern.xsd");
            XPathNodeIterator nodes = nav.Select("act:accordings/action", manager);
            while (nodes.MoveNext())
            {
                XPathNavigator actNav = nodes.Current;
                string actionID = actNav.GetAttribute("actionID", "");
                INotifyAction action = _ActionProvider.GetAction(actionID);
                if (action == null)
                {
                    LogHolder.Log("ASC.Notify").Error(String.Format("action with id=\"{0}\" not instanced", actionID));
                    continue;
                }
                string defaultActionPattern = actNav.GetAttribute("defaultPatternID", "");
                if (!String.IsNullOrEmpty(defaultActionPattern))
                {
                    IPattern defaultPattern = _PatternProvider.GetPattern(defaultActionPattern);
                    if (defaultPattern == null)
                    {
                        LogHolder.Log("ASC.Notify").Error(String.Format("pattern with id=\"{0}\" not instanced",
                                                                        defaultActionPattern));
                        continue;
                    }

                    result.Add(new TernarAccord(action, new Accord(null, defaultPattern)));
                }
                XPathNodeIterator sendersNodes = actNav.SelectChildren("sender", "");
                while (sendersNodes.MoveNext())
                {
                    string senderName = sendersNodes.Current.GetAttribute("senderName", "");
                    string patternID = sendersNodes.Current.GetAttribute("patternID", "");
                    IPattern pattern = _PatternProvider.GetPattern(patternID);
                    if (pattern == null)
                    {
                        LogHolder.Log("ASC.Notify").Error(String.Format("pattern with id=\"{0}\" not instanced",
                                                                        patternID));
                        continue;
                    }
                    result.Add(new TernarAccord(action, new Accord(senderName, pattern)));
                }
            }
            _ConstProvider = new ConstActionPatternProvider(result.ToArray());
        }
    }
}