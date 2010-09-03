using System;

namespace AmazonS3Commander
{
    static class EnumExt
    {
        public static bool IsSet(this Enum en, Enum flag)
        {
            var flag1 = ((IConvertible)en).ToInt64(null);
            var flag2 = ((IConvertible)flag).ToInt64(null);
            return flag2 == (flag1 & flag2);
        }
    }
}
