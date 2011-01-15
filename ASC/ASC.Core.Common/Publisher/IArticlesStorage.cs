#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IArticlesStorage
    {
        string ID { get; }

        string[] Languages { get; }

        DateTime Date { get; }

        string[] Countries { get; }

        Version PublisherVersion { get; }

        List<IDBArticle> GetArticles(string zone);

        IDBArticle GetArticle(string id);
    }
}