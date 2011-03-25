using System;
using System.Security.Principal;

namespace ASC.Common.Security.Authorizing
{
    public interface ISubject : IIdentity
    {
        Guid ID { get; }
    }
}