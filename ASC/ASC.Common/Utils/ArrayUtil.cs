#region usings

using System;
using System.Collections.Generic;
using ASC.Reflection;

#endregion

namespace ASC.Common.Utils
{
    public class ArrayUtil
    {
        public static T[] CopyClonable<T>(T[] source)
        {
            return (T[]) CopyClonable(source, typeof (T));
        }

        public static T[] CopyClonable<T>(IList<T> source)
        {
            if (source == null) return null;
            var src = new T[source.Count];
            source.CopyTo(src, 0);
            return (T[]) CopyClonable(src, typeof (T));
        }

        internal static Array CopyClonable(Array source, Type ofType)
        {
            if (source == null) return null;

            if (!TypeHelper.ImplementInterface(ofType, typeof (ICloneable)))
            {
                return (Array) source.Clone();
            }

            bool isArray = TypeHelper.IsSameOrParent(typeof (Array), ofType);
            Array dest = Array.CreateInstance(ofType, source.Length);
            Array sourceElement = null;
            if (source.Length > 0)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (isArray)
                    {
                        sourceElement = (source.GetValue(i) as Array);
                        if (sourceElement.Length > 0 && sourceElement.GetValue(0) != null)
                            dest.SetValue(
                                CopyClonable(source.GetValue(i) as Array, sourceElement.GetValue(0).GetType()), i);
                        else
                            dest.SetValue(CopyClonable(source.GetValue(i) as Array, typeof (object)), i);
                    }
                    else
                    {
                        dest.SetValue((source.GetValue(i) as ICloneable).Clone(), i);
                    }
                }
            }
            return dest;
        }
    }
}