#region usings

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace ASC.Notify.Model.Serialization
{
    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:asc.notify.action_pattern.xsd")]
    [XmlRoot(Namespace = "urn:asc.notify.action_pattern.xsd", IsNullable = false)]
    public class accordings
    {
        private accord[] actionField;

        [XmlElement("action", Form = XmlSchemaForm.Unqualified)]
        public accord[] action
        {
            get { return actionField; }
            set { actionField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.action_pattern.xsd")]
    public class accord
    {
        private string actionIDField;

        private string defaultPatternIDField;
        private sender[] patternField;

        [XmlElement("pattern", Form = XmlSchemaForm.Unqualified)]
        public sender[] pattern
        {
            get { return patternField; }
            set { patternField = value; }
        }

        [XmlAttribute]
        public string actionID
        {
            get { return actionIDField; }
            set { actionIDField = value; }
        }

        [XmlAttribute]
        public string defaultPatternID
        {
            get { return defaultPatternIDField; }
            set { defaultPatternIDField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.action_pattern.xsd")]
    public class sender
    {
        private string patternIDField;
        private string senderNameField;

        [XmlAttribute]
        public string senderName
        {
            get { return senderNameField; }
            set { senderNameField = value; }
        }

        [XmlAttribute]
        public string patternID
        {
            get { return patternIDField; }
            set { patternIDField = value; }
        }
    }
}