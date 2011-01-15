#region usings

using System.Collections;

#endregion

namespace ASC.Notify.Cron
{

    #region

    #endregion

    public interface ISet : ICollection, IList
    {
        #region Methods

        new bool Add(object obj);

        bool AddAll(ICollection c);

        object First();

        #endregion
    }
}