#region usings

using System;
using System.Security.Principal;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public interface ISubject : IIdentity
    {
        Guid ID { get; }
    }
}