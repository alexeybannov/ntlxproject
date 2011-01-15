#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using ASC.Common.Utils;
using ASC.Core.Users;

#endregion

namespace ASC.Core.Common.Publisher
{
    public abstract class PublisherBase
        : IPublisher
    {
        protected readonly object SyncRoot = new object();

        protected PublisherBase(
            Version version,
            Version publisherVersion
            )
        {
            Version = version;
            PublisherVersion = publisherVersion;
        }

        #region IPublisher

        public Version Version { get; protected set; }

        public Version PublisherVersion { get; protected set; }

        public Hashtable Properties { get; protected set; }

        public virtual void Initialize(Hashtable properties)
        {
            Properties = properties ?? new Hashtable();
            AfterInitialize();
        }

        public virtual List<Article> HandleRequest(RequestContext context, List<Zone> visibleZones)
        {
            using (DebugUtil.Watch("publisher: handling request"))
            {
                using (DebugUtil.Watch("publisher: validatind article db"))
                {
                    ValidateArticleDB();
                }
                var result = new List<Article>();
                foreach (Zone zone in visibleZones ?? new List<Zone>())
                {
                    Article zoneArticle = null;
                    try
                    {
                        zoneArticle = HandleRequest(context, zone);
                    }
                    catch (Exception exc)
                    {
                        LogHolder.Log("ASC.Publisher")
                            .Error(String.Format("failed handle zone {0}", zone.ID), exc);
                    }
                    if (zoneArticle != null)
                        result.Add(zoneArticle);
                }
                return result;
            }
        }

        #endregion

        protected Random Rand = new Random(DateTime.Now.Millisecond);

        public abstract IArticlesStorage ArticlesStorage { get; }

        public abstract IPublisherStorage PublisherStorage { get; }

        protected abstract void AfterInitialize();

        protected virtual Article HandleRequest(RequestContext context, Zone zone)
        {
            List<IDBArticle> articles = null;
            lock (SyncRoot)
            {
                articles = ArticlesStorage.GetArticles(zone.ID);
            }
            Article forShow = null;
            if (articles != null && articles.Count > 0)
            {
                using (DebugUtil.Watch(String.Format("publisher: choose from {0} for {1}", articles.Count, zone.ID)))
                {
                    var notLimited = new List<IDBArticle>();
                    var forChoose = new Dictionary<IDBArticle, List<IDBTarget>>();
                    foreach (IDBArticle article in articles ?? new List<IDBArticle>())
                    {
                        if (ReachLimit(context, article)) continue;
                        List<IDBTarget> reachedTargets = ReachTarget(context, article);
                        if (reachedTargets == null || reachedTargets.Count == 0) continue;
                        forChoose.Add(article, reachedTargets);
                    }
                    IDBArticle winner = SelectArticle(context, forChoose);
                    if (winner != null && winner.Type != ArticleType.Blank)
                    {
                        string html = PrepareContent(context, winner);
                        forShow = new Article(winner.ID, zone, winner.Type, html);
                    }
                }
                using (DebugUtil.Watch("publisher: save show"))
                {
                    PublisherStorage.ShowArticle(context, forShow, zone.ID);
                }
            }
            return forShow;
        }

        protected abstract bool ReachLimit(RequestContext context, IDBArticle article);

        protected abstract List<IDBTarget> ReachTarget(RequestContext context, IDBArticle article);

        protected virtual IDBArticle SelectArticle(RequestContext context,
                                                   Dictionary<IDBArticle, List<IDBTarget>> reachedTargets)
        {
            int summ = 0;
            foreach (KeyValuePair<IDBArticle, List<IDBTarget>> arttargets in reachedTargets)
                arttargets.Value.ForEach((target) => summ += target.Priority);
            int rnd = Rand.Next(summ);
            summ = 0;
            IDBArticle winner = null;
            foreach (KeyValuePair<IDBArticle, List<IDBTarget>> arttargets in reachedTargets)
            {
                foreach (IDBTarget target in arttargets.Value)
                {
                    if (rnd >= summ && rnd < summ + target.Priority)
                        winner = arttargets.Key;
                    if (winner != null) break;
                    summ += target.Priority;
                }
                if (winner != null) break;
            }
            return winner;
        }

        protected virtual string PrepareContent(RequestContext context, IDBArticle article)
        {
            return article.Content.Replace("{{username}}", context.User.DisplayName());
        }

        protected abstract void ValidateArticleDB();
    }
}