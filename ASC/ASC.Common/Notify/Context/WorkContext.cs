#region usings

using System;
using ASC.Notify.Channels;
using ASC.Notify.Engine;

#endregion

namespace ASC.Notify
{
    internal static class WorkContext
    {
        internal static readonly object SyncRoot = new object();

        #region

        private static Context _Context;

        public static Context Context
        {
            get
            {
                if (_Context == null)
                    lock (SyncRoot)
                    {
                        if (_Context == null)
                            _Context = new Context();
                    }
                return _Context;
            }
        }

        [Obsolete]
        public static SenderHolder SenderHolder
        {
            get { return Context.SenderHolder; }
        }

        [Obsolete]
        public static INotifyService NotifyService
        {
            get { return Context.NotifyService; }
        }

        [Obsolete]
        internal static NotifyEngine NotifyEngine
        {
            get { return Context.NotifyEngine; }
        }

        #endregion
    }
}