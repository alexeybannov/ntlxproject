#region usings

using System;

#endregion

namespace ASC.Common.Security
{
    public interface ISecurityObjectId
    {
        object SecurityId { get; }
        Type ObjectType { get; }
    }
}