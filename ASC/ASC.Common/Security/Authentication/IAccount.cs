#region usings

using System;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security.Authentication
{
    public interface IAccount : ISubject, ICloneable
    {
        SecurityLevel SecurityLevel { get; }
    }
}