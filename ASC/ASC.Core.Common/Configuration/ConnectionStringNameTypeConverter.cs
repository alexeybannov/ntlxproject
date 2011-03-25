using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace ASC.Core.Common.Configuration
{
    class ConnectionStringNameTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return ConfigurationManager.ConnectionStrings[(string)value];
        }
    }
}
