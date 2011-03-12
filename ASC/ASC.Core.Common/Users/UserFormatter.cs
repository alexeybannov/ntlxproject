using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace ASC.Core.Users
{
    public class UserFormatter : IComparer<UserInfo>
    {
        private readonly DisplayUserNameFormat format;


        public UserFormatter()
            : this(DisplayUserNameFormat.Default)
        {
        }

        public UserFormatter(DisplayUserNameFormat format)
        {
            this.format = format;
        }


        public static string GetUserName(UserInfo userInfo, DisplayUserNameFormat format)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            return string.Format(GetUserDisplayFormat(format), userInfo.FirstName, userInfo.LastName);
        }

        public static string GetUserName(UserInfo userInfo)
        {
            return GetUserName(userInfo, DisplayUserNameFormat.Default);
        }

        int IComparer<UserInfo>.Compare(UserInfo x, UserInfo y)
        {
            return Compare(x, y, format);
        }

        public static int Compare(UserInfo x, UserInfo y)
        {
            return Compare(x, y, DisplayUserNameFormat.Default);
        }

        public static int Compare(UserInfo x, UserInfo y, DisplayUserNameFormat format)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return -1;
            if (x != null && y == null) return +1;

            var result = 0;
            if (format == DisplayUserNameFormat.Default) format = GetUserDisplayDefaultOrder();
            if (format == DisplayUserNameFormat.FirstLast)
            {
                result = String.Compare(x.FirstName, y.FirstName, true);
                if (result == 0) result = String.Compare(x.LastName, y.LastName, true);
            }
            else
            {
                result = String.Compare(x.LastName, y.LastName, true);
                if (result == 0) result = String.Compare(x.FirstName, y.FirstName, true);
            }
            return result;
        }

        private static readonly Dictionary<string, Dictionary<DisplayUserNameFormat, string>> DisplayFormats = new Dictionary<string, Dictionary<DisplayUserNameFormat, string>>
        {
            { "ru", new Dictionary<DisplayUserNameFormat, string>{ { DisplayUserNameFormat.Default, "{1} {0}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1} {0}" } } },
            { "en", new Dictionary<DisplayUserNameFormat, string>{ { DisplayUserNameFormat.Default, "{0} {1}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1}, {0}" } } },
            { "default", new Dictionary<DisplayUserNameFormat, string>{ {DisplayUserNameFormat.Default, "{0} {1}" }, { DisplayUserNameFormat.FirstLast, "{0} {1}" }, { DisplayUserNameFormat.LastFirst, "{1}, {0}" } }
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
                if (String.IsNullOrEmpty(_forceFormat)) _forceFormat = null;
                _forceFormatChecked = true;
            }
            if (_forceFormat != null) return _forceFormat;
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            Dictionary<DisplayUserNameFormat, string> formats = null;
            if (!DisplayFormats.TryGetValue(culture, out formats))
            {
                var twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats)) formats = DisplayFormats["default"];
            }
            return formats[format];
        }

        public static DisplayUserNameFormat GetUserDisplayDefaultOrder()
        {
            var culture = Thread.CurrentThread.CurrentCulture.Name;
            Dictionary<DisplayUserNameFormat, string> formats = null;
            if (!DisplayFormats.TryGetValue(culture, out formats))
            {
                string twoletter = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                if (!DisplayFormats.TryGetValue(twoletter, out formats)) formats = DisplayFormats["default"];
            }
            var format = formats[DisplayUserNameFormat.Default];
            return format.IndexOf("{0}") < format.IndexOf("{1}") ? DisplayUserNameFormat.FirstLast : DisplayUserNameFormat.LastFirst;
        }
    }
}