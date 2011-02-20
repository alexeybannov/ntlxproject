#region usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

#endregion

namespace ASC.Core.Users
{
    public class UserFormatter
        : IComparer<UserInfo>
    {
        private readonly DisplayUserNameFormat _format = DisplayUserNameFormat.Default;

        public UserFormatter()
        {
        }

        public UserFormatter(DisplayUserNameFormat format)
        {
            _format = format;
        }

        #region display users

        public static string GetUserName(UserInfo userInfo, DisplayUserNameFormat format)
        {
            if (userInfo == null)
                throw new ArgumentNullException("userInfo");
            return String.Format(GetUserDisplayFormat(format), userInfo.FirstName, userInfo.LastName);
        }

        public static string GetUserName(UserInfo userInfo)
        {
            return GetUserName(userInfo, DisplayUserNameFormat.Default);
        }

        #endregion

        #region sort users

        #region IComparer<UserInfo> Members

        int IComparer<UserInfo>.Compare(UserInfo x, UserInfo y)
        {
            return Compare(x, y, _format);
        }

        #endregion

        public static int Compare(UserInfo x, UserInfo y)
        {
            return Compare(x, y, DisplayUserNameFormat.Default);
        }

        public static int Compare(UserInfo x, UserInfo y, DisplayUserNameFormat format)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return -1;
            if (x != null && y == null) return +1;
            int result = 0;
            if (format == DisplayUserNameFormat.Default)
                format = GetUserDisplayDefaultOrder();
            if (format == DisplayUserNameFormat.FirstLast)
            {
                result = String.Compare(x.FirstName, y.FirstName, true);
                if (result == 0)
                    result = String.Compare(x.LastName, y.LastName, true);
            }
            else
            {
                result = String.Compare(x.LastName, y.LastName, true);
                if (result == 0)
                    result = String.Compare(x.FirstName, y.FirstName, true);
            }
            return result;
        }

        #endregion

        private static readonly Dictionary<string, Dictionary<DisplayUserNameFormat, string>> DisplayFormats = new Dictionary
            <string, Dictionary<DisplayUserNameFormat, string>>
                                                                                                                   {
                                                                                                                       {
                                                                                                                           "ru"
                                                                                                                           ,
                                                                                                                           new Dictionary
                                                                                                                           <
                                                                                                                           DisplayUserNameFormat
                                                                                                                           ,
                                                                                                                           string
                                                                                                                           >
                                                                                                                               {
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       Default
                                                                                                                                       ,
                                                                                                                                       "{1} {0}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       FirstLast
                                                                                                                                       ,
                                                                                                                                       "{0} {1}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       LastFirst
                                                                                                                                       ,
                                                                                                                                       "{1} {0}"
                                                                                                                                       }
                                                                                                                               }
                                                                                                                           },
                                                                                                                       {
                                                                                                                           "en"
                                                                                                                           ,
                                                                                                                           new Dictionary
                                                                                                                           <
                                                                                                                           DisplayUserNameFormat
                                                                                                                           ,
                                                                                                                           string
                                                                                                                           >
                                                                                                                               {
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       Default
                                                                                                                                       ,
                                                                                                                                       "{0} {1}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       FirstLast
                                                                                                                                       ,
                                                                                                                                       "{0} {1}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       LastFirst
                                                                                                                                       ,
                                                                                                                                       "{1}, {0}"
                                                                                                                                       }
                                                                                                                               }
                                                                                                                           },
                                                                                                                       {
                                                                                                                           "default"
                                                                                                                           ,
                                                                                                                           new Dictionary
                                                                                                                           <
                                                                                                                           DisplayUserNameFormat
                                                                                                                           ,
                                                                                                                           string
                                                                                                                           >
                                                                                                                               {
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       Default
                                                                                                                                       ,
                                                                                                                                       "{0} {1}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       FirstLast
                                                                                                                                       ,
                                                                                                                                       "{0} {1}"
                                                                                                                                       },
                                                                                                                                   {
                                                                                                                                       DisplayUserNameFormat
                                                                                                                                       .
                                                                                                                                       LastFirst
                                                                                                                                       ,
                                                                                                                                       "{1}, {0}"
                                                                                                                                       }
                                                                                                                               }
                                                                                                                           }
                                                                                                                   };

        private static string GetUserDisplayFormat()
        {
            return GetUserDisplayFormat(DisplayUserNameFormat.Default);
        }

        private static bool _forceFormatChecked;
        private static string _forceFormat;

        private static string GetUserDisplayFormat(DisplayUserNameFormat format)
        {
            if (!_forceFormatChecked)
            {
                _forceFormat = ConfigurationManager.AppSettings["asc.core.users.user-display-format"];
                if (String.IsNullOrEmpty(_forceFormat))
                    _forceFormat = null;
                _forceFormatChecked = true;
            }
            if (_forceFormat != null) return _forceFormat;
            string culture = Thread.CurrentThread.CurrentCulture.Name;
            Dictionary<DisplayUserNameFormat, string> formats = null;
            if (!DisplayFormats.TryGetValue(culture, out formats))
            {
                string twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats))
                    formats = DisplayFormats["default"];
            }
            return formats[format];
        }

        public static DisplayUserNameFormat GetUserDisplayDefaultOrder()
        {
            string culture = Thread.CurrentThread.CurrentCulture.Name;
            Dictionary<DisplayUserNameFormat, string> formats = null;
            if (!DisplayFormats.TryGetValue(culture, out formats))
            {
                string twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats))
                    formats = DisplayFormats["default"];
            }
            string format = formats[DisplayUserNameFormat.Default];
            return format.IndexOf("{0}") < format.IndexOf("{1}")
                       ? DisplayUserNameFormat.FirstLast
                       : DisplayUserNameFormat.LastFirst;
        }
    }
}