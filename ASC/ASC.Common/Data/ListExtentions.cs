#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Data
{
    public static class ListExtentions
    {
        public static List<TObject> MapToObject<TObject>(this List<object[]> list,
                                                         Converter<object[], TObject> converter)
        {
            return list.ConvertAll(converter);
        }

        public static List<T> ForEach<T>(this List<T> list, Action<T> action)
        {
            list.ForEach(action);
            return list;
        }

        public static List<T> Sort<T>(this List<T> list)
        {
            list.Sort();
            return list;
        }

        public static List<T> RemoveAll<T>(this List<T> list, Predicate<T> match)
        {
            list.RemoveAll(match);
            return list;
        }
    }
}