#region usings

using System;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IDBArticle
    {
        string ID { get; }

        ArticleType Type { get; }

        string[] Zones { get; }

        DateTime? Date { get; }

        IDBTarget[] Targets { get; }

        IDBLimit[] Limits { get; }

        string Content { get; }
    }
}