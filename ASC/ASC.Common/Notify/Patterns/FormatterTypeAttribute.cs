#region usings

using System;
using System.Reflection;

#endregion

namespace ASC.Notify.Patterns
{
    public class FormatterTypeAttribute
        : Attribute
    {
        public FormatterTypeAttribute(Type formatterType)
        {
            SetFormatterType(formatterType);
        }

        public Type FormatterType { get; set; }

        public IPatternFormatter CreateFormatter()
        {
            if (FormatterType == null) throw new ApplicationException("formatter type not set");
            ConstructorInfo ctorInfo = FormatterType.GetConstructor(null);
            if (ctorInfo == null) throw new ApplicationException("invalid formatter type");
            return (IPatternFormatter) ctorInfo.Invoke(null);
        }

        private void SetFormatterType(Type formatterType)
        {
            if (formatterType == null) throw new ArgumentNullException("formatterType");
            FormatterType = formatterType;
        }
    }
}