namespace ASC.Notify.Cron
{
    public interface ISortedSet : ISet
    {
        #region Methods

        ISortedSet TailSet(object limit);

        #endregion
    }
}