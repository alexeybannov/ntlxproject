#region usings

using System;

#endregion

namespace ASC.Core.Common.Publisher
{
    public class Article
    {
        public Article(string id, Zone zone, ArticleType type, string html)
        {
            if (zone == null) throw new ArgumentNullException("zone");
            Zone = zone;
            Type = type;
            Html = html;
            ID = id;
        }

        public Article(Zone zone, ArticleType type, string html)
            : this(null, zone, type, html)
        {
        }

        public string ID { get; internal set; }
        public Zone Zone { get; internal set; }
        public ArticleType Type { get; internal set; }
        public string Html { get; internal set; }
    }
}