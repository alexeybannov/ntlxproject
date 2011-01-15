#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    [AttributeUsage(
        AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public class AuthenticationLevelAttribute
        : Attribute
    {
        public AuthenticationLevelAttribute(SecurityLevel level)
        {
            Level = level;
        }

        public SecurityLevel Level { get; internal set; }
    }
}