#region usings

using System;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IPublisherStorage
    {
        void ShowArticle(RequestContext context, Article forShow, string zoneid);

        int GetArticleShows(string art, string userid, DateTime? fromDate, DateTime? toDate);

        DateTime? GetLastShow(string art, string userid);

        void SaveArticleDB(string id, DateTime date, string content);

        string GetCurrentArticleDB();

        DateTime GetLastSuccesfullSyncDate();

        void SaveLastSuccesfullSyncDate(DateTime date);
    }
}