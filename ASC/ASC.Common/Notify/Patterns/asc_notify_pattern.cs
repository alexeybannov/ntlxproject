#region usings

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace ASC.Notify.Patterns.Serialization
{
    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:asc.notify.pattern.xsd")]
    [XmlRoot(Namespace = "urn:asc.notify.pattern.xsd", IsNullable = false)]
    public class catalog
    {
        private patternsBlock[] blockField;

        [XmlElement("block", Form = XmlSchemaForm.Unqualified)]
        public patternsBlock[] block
        {
            get { return blockField; }
            set { blockField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class patternsBlock
    {
        private string contentTypeField;
        private formatterType formatterField;

        private patternsList patternsField;

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public formatterType formatter
        {
            get { return formatterField; }
            set { formatterField = value; }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public patternsList patterns
        {
            get { return patternsField; }
            set { patternsField = value; }
        }

        [XmlAttribute]
        public string contentType
        {
            get { return contentTypeField; }
            set { contentTypeField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class formatterType
    {
        private string typeField;

        [XmlAttribute]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class tagType
    {
        private string nameField;

        [XmlAttribute]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class useTagsType
    {
        private tagType[] tagField;

        [XmlElement("tag", Form = XmlSchemaForm.Unqualified)]
        public tagType[] tag
        {
            get { return tagField; }
            set { tagField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class patternType
    {
        private string bodyField;
        private string contentTypeField;

        private string idField;

        private string nameField;
        private string subjectField;
        private useTagsType useTagsField;

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string subject
        {
            get { return subjectField; }
            set { subjectField = value; }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string body
        {
            get { return bodyField; }
            set { bodyField = value; }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public useTagsType useTags
        {
            get { return useTagsField; }
            set { useTagsField = value; }
        }

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

        [XmlAttribute]
        public string contentType
        {
            get { return contentTypeField; }
            set { contentTypeField = value; }
        }
    }

    [GeneratedCode("xsd", "2.0.50727.312")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "urn:asc.notify.pattern.xsd")]
    public class patternsList
    {
        private patternType[] patternField;

        [XmlElement("pattern", Form = XmlSchemaForm.Unqualified)]
        public patternType[] pattern
        {
            get { return patternField; }
            set { patternField = value; }
        }
    }
}