#region usings

using System;

#endregion

namespace ASC.Notify.Patterns
{
    public class Pattern
        : IPattern
    {
        #region

        public const string HTMLContentType = "html";

        public const string TextContentType = "text";

        public const string RtfContentType = "rtf";

        #endregion

        public Pattern(string id, string name, string subject, string body)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("id");
            if (subject == null) throw new ArgumentNullException("subject");
            if (body == null) throw new ArgumentNullException("body");
            ID = id;
            Name = name;
            Subject = subject;
            Body = body;
            ContentType = TextContentType;
        }

        public Pattern(string id, string name, string subject, string body, string contentType)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("id");
            if (subject == null) throw new ArgumentNullException("subject");
            if (body == null) throw new ArgumentNullException("body");
            ID = id;
            Name = name;
            Subject = subject;
            Body = body;
            ContentType = !String.IsNullOrEmpty(contentType) ? contentType : TextContentType;
        }

        #region IPattern

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public string ContentType { get; internal set; }

        #endregion

        public override bool Equals(object obj)
        {
            var pattern = obj as IPattern;
            if (pattern == null) return false;
            return pattern.ID == ID;
        }

        public override int GetHashCode()
        {
            return (ID ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", ID, Name);
        }
    }
}