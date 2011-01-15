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
    [XmlType(AnonymousType = true, Namespace = "urn:asc.notify.action.xsd")]
    [XmlRoot(Namespace = "urn:asc.notify.action.xsd", IsNullable = false)]
    public class catalog
    {
        private actionType[] actionField;

        [XmlElement("action", Form = XmlSchemaForm.Unqualified)]
        public actionType[] action
        {
            get { return actionField; }
            set { actionField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.action.xsd")]
    public class actionType
    {
        private string idField;

        private string nameField;

        [XmlAttribute]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        [XmlAttribute]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }
}