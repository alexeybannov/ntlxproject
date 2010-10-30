using System;
using System.Xml;

namespace LitS3
{
    /// <summary>
    /// Represents an Amazon S3 user.
    /// </summary>
    public class Identity
    {
        public string Id
        {
            get;
            protected set;
        }

        public string DisplayName
        {
            get;
            protected set;
        }


        public Identity(string id, string displayName)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            Id = id;
            DisplayName = displayName;
        }

        protected Identity()
        {

        }

        internal Identity(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                throw new Exception("Expected a non-empty <Owner> element.");

            // Example:
            // <Owner>
            //     <ID>bcaf1ffd86f41caff1a493dc2ad8c2c281e37522a640e161ca5fb16fd081034f</ID>
            //     <DisplayName>webfile</DisplayName>
            // </Owner>
            reader.ReadStartElement("Owner");
            this.Id = reader.ReadElementContentAsString("ID", "");
            this.DisplayName = reader.ReadElementContentAsString("DisplayName", "");
            reader.ReadEndElement();
        }


        public override bool Equals(object obj)
        {
            var identity = obj as Identity;
            return identity != null && identity.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} (Owner)", DisplayName ?? Id);
        }
    }
}
