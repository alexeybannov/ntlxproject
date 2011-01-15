#region usings

using System;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Engine
{
    internal sealed class ProviderResolver
    {
        public static T Get<T>(INotifySource source) where T : class
        {
            return GetInternal<T>(source);
        }

        public static T GetEnsure<T>(INotifySource source) where T : class
        {
            var result = GetInternal<T>(source);
            if (result == null)
            {
                throw new NotifyException(String.Format(Resource.NotifyException_Message_ProviderNotInstanced,
                                                        typeof (T).Name));
            }
            return result;
        }

        private static T GetInternal<T>(INotifySource source) where T : class
        {
            T result = null;
            if (source == null) return null;
            if (typeof (T) == typeof (IActionPatternProvider))
                result = (T) WrappedGet(() => source.GetActionPatternProvider());
            if (typeof (T) == typeof (IActionProvider))
                result = (T) WrappedGet(() => source.GetActionProvider());
            if (typeof (T) == typeof (IPatternProvider))
                result = (T) WrappedGet(() => source.GetPatternProvider());
            if (typeof (T) == typeof (IDependencyProvider))
                result = (T) WrappedGet(() => source.GetDependencyProvider());
            if (typeof (T) == typeof (ISubscriptionProvider))
                result = (T) WrappedGet(() => source.GetSubscriptionProvider());
            if (typeof (T) == typeof (IRecipientProvider))
                result = (T) WrappedGet(() => source.GetRecipientsProvider());
            if (typeof (T) == typeof (ISubscriptionSource))
                result = (T) WrappedGet(() => source.GetSubscriptionSource());
            return result;
        }

        private static T WrappedGet<T>(GetInt<T> getdelegate) where T : class
        {
            try
            {
                return getdelegate();
            }
            catch
            {
                return default(T);
            }
        }

        #region Nested type: GetInt

        private delegate T GetInt<T>() where T : class;

        #endregion
    }
}