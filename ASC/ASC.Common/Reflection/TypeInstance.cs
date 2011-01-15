#region usings

using System;
using System.Reflection;

#endregion

namespace ASC.Reflection
{
    public sealed class TypeInstance
    {
        public static T Create<T>() where T : class
        {
            return (T) Create(typeof (T));
        }

        public static object Create(Type T)
        {
            if (T == null)
                throw new ArgumentNullException("T");
            ConstructorInfo ctorInfo = TypeHelper.GetDefaultConstructor(T);
            if (ctorInfo == null)
                throw new ArgumentException(String.Format("Type \"{0}\" hasn't default contructor", T));
            return ctorInfo.Invoke(null);
        }

        public static T Create<T>(params object[] ctorParams)
            where T : class
        {
            return Create(typeof (T), ctorParams) as T;
        }

        public static object Create(Type T, params object[] ctorParams)
        {
            if (T == null)
                throw new ArgumentNullException("T");
            if (ctorParams == null || ctorParams.Length == 0)
                return Create(T);

            var ctorParamsType = new Type[ctorParams.Length];
            for (int i = 0; i < ctorParams.Length; i++)
            {
                if (ctorParams[i] == null)
                    throw new ArgumentNullException(String.Format("ctorParams[{0}]", i));
                ctorParamsType[i] = ctorParams[i].GetType();
            }
            ConstructorInfo ctorInfo = TypeHelper.GetConstructor(T, ctorParamsType);
            if (ctorInfo == null)
                throw new ArgumentException(String.Format("Type \"{0}\" hasn't appropriate contructor", T));
            return ctorInfo.Invoke(ctorParams);
        }
    }
}