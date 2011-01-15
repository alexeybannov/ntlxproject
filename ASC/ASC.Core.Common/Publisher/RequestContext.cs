#region usings

using System;
using ASC.Core.Users;

#endregion

namespace ASC.Core.Common.Publisher
{
    public class RequestContext
    {
        public RequestContext()
        {
            DateTime = DateTime.Now;
        }

        public DateTime DateTime { get; set; }

        public Guid ProductID { get; set; }

        public Guid ModuleID { get; set; }

        public UserInfo User { get; set; }
    }
}