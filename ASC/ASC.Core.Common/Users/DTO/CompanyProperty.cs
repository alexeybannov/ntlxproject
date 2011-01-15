#region usings

using System;
using System.Diagnostics;

#endregion

namespace ASC.Core.Users
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    [Serializable]
    public class CompanyProperty
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public CompanyProperty(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Name = name;
            Value = value;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var p = obj as CompanyProperty;
            return p != null && Name.Equals(p.Name);
        }
    }
}