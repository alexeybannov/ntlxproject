#region usings

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

#endregion

namespace ASC.Reflection
{
    [DebuggerDisplay("Type = {Type}")]
    public class TypeHelper
    {
        private readonly Type _type;

        public TypeHelper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
        }

        public Type Type
        {
            get { return _type; }
        }

        public static implicit operator TypeHelper(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return new TypeHelper(type);
        }

        public static implicit operator Type(TypeHelper typeHelper)
        {
            if (typeHelper == null) throw new ArgumentNullException("typeHelper");
            return typeHelper.Type;
        }

        public static bool IsNullable(Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>));
        }

        public static Type GetUnderlyingType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (IsNullable(type))
                type = type.GetGenericArguments()[0];
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            return type;
        }

        public static bool IsSameOrParent(Type parent, Type child)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");
            if (parent == child ||
                child.IsEnum && Enum.GetUnderlyingType(child) == parent ||
                child.IsSubclassOf(parent))
            {
                return true;
            }

            if (parent.IsInterface)
                return ImplementInterface(child, parent);
            return false;
        }

        public static bool ImplementInterface(Type type, Type interfaceType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            if (interfaceType.IsInterface)
            {
                Type[] interfaces = type.GetInterfaces();
                foreach (Type t in interfaces)
                    if (t == interfaceType)
                        return true;
            }
            return false;
        }

        public static MethodInfo GetMethod(Type type, bool generic, string methodName, BindingFlags flags)
        {
            if (type == null) throw new ArgumentNullException("type");
            foreach (MethodInfo method in type.GetMethods(flags))
            {
                if (method.IsGenericMethodDefinition == generic && method.Name == methodName)
                    return method;
            }
            return null;
        }

        public static MethodInfo[] GetMethods(Type type, bool generic, BindingFlags flags)
        {
            if (type == null) throw new ArgumentNullException("type");
            return Array.FindAll(
                type.GetMethods(flags),
                delegate(MethodInfo method) { return method.IsGenericMethodDefinition == generic; });
        }

        public static MethodInfo GetMethod(
            Type type,
            string methodName,
            BindingFlags bindingFlags,
            int requiredParametersCount,
            params Type[] parameterTypes)
        {
            while (parameterTypes.Length >= requiredParametersCount)
            {
                MethodInfo method = type.GetMethod(methodName, parameterTypes);
                if (null != method)
                    return method;
                if (parameterTypes.Length == 0)
                    break;
                Array.Resize(ref parameterTypes, parameterTypes.Length - 1);
            }
            return null;
        }

        public static PropertyInfo GetPropertyInfo(
            Type type, string propertyName, Type returnType, Type[] types)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                returnType,
                types,
                null);
        }

        public static Type GetListItemType(object list)
        {
            Type typeOfObject = typeof (object);
            if (list == null)
                return typeOfObject;

            if (list is Array)
                return list.GetType().GetElementType();
            Type type = list.GetType();

            if (list is IList || list is ITypedList || list is IListSource)
            {
                PropertyInfo last = null;
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (pi.GetIndexParameters().Length > 0 && pi.PropertyType != typeOfObject)
                    {
                        if (pi.Name == "Item")
                            return pi.PropertyType;
                        last = pi;
                    }
                }
                if (last != null)
                    return last.PropertyType;
            }
            try
            {
                if (list is IList)
                {
                    var l = (IList) list;
                    for (int i = 0; i < l.Count; i++)
                    {
                        object o = l[i];
                        if (o != null && o.GetType() != typeOfObject)
                            return o.GetType();
                    }
                }
                else if (list is IEnumerable)
                {
                    foreach (object o in (IEnumerable) list)
                    {
                        if (o != null && o.GetType() != typeOfObject)
                            return o.GetType();
                    }
                }
            }
            catch
            {
            }
            return typeOfObject;
        }

        public static Type GetListItemType(Type listType)
        {
            if (listType.IsGenericType)
            {
                Type[] elementTypes = GetGenericArguments(listType, typeof (IList));
                if (elementTypes != null)
                    return elementTypes[0];
            }
            if (IsSameOrParent(typeof (IList), listType) ||
                IsSameOrParent(typeof (ITypedList), listType) ||
                IsSameOrParent(typeof (IListSource), listType))
            {
                Type elementType = listType.GetElementType();
                if (elementType != null)
                    return elementType;
                PropertyInfo last = null;
                foreach (PropertyInfo pi in listType.GetProperties())
                {
                    if (pi.GetIndexParameters().Length > 0 && pi.PropertyType != typeof (object))
                    {
                        if (pi.Name == "Item")
                            return pi.PropertyType;
                        last = pi;
                    }
                }
                if (last != null)
                    return last.PropertyType;
            }
            return typeof (object);
        }

        public static bool IsScalar(Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();
            return type.IsValueType
                   || type == typeof (string)
                   || type == typeof (Stream)
                   || type == typeof (XmlReader)
                   || type == typeof (XmlDocument);
        }

        public static Type[] GetGenericArguments(Type type, Type baseType)
        {
            string baseTypeName = baseType.Name;
            for (Type t = type; t != typeof (object) && t != null; t = t.BaseType)
                if (t.IsGenericType && (baseTypeName == null || t.Name.Split('`')[0] == baseTypeName))
                    return t.GetGenericArguments();
            foreach (Type t in type.GetInterfaces())
                if (t.IsGenericType && (baseTypeName == null || t.Name.Split('`')[0] == baseTypeName))
                    return t.GetGenericArguments();
            return null;
        }

        public static Type TranslateGenericParameters(Type type, Type[] typeArguments)
        {
            if (type.IsGenericParameter)
                return typeArguments[type.GenericParameterPosition];

            if (type.IsGenericType && type.ContainsGenericParameters)
            {
                Type[] genArgs = type.GetGenericArguments();
                for (int i = 0; i < genArgs.Length; ++i)
                    genArgs[i] = TranslateGenericParameters(genArgs[i], typeArguments);
                return type.GetGenericTypeDefinition().MakeGenericType(genArgs);
            }

            return type;
        }

        #region GetAttributes

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _type.GetCustomAttributes(attributeType, inherit);
        }

        public object[] GetCustomAttributes(Type attributeType)
        {
            return _type.GetCustomAttributes(attributeType, true);
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return _type.GetCustomAttributes(inherit);
        }

        public object[] GetCustomAttributes()
        {
            return _type.GetCustomAttributes(true);
        }

        public object[] GetAttributes(Type attributeType)
        {
            return GetAttributes(_type, attributeType);
        }

        public object[] GetAttributes()
        {
            return GetAttributesInternal();
        }

        public static object[] GetAttributes(Type type, Type attributeType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (attributeType == null) throw new ArgumentNullException("attributeType");
            string key = type.FullName + "#" + attributeType.FullName;
            var attrs = (object[]) _typeAttributes[key];
            if (attrs == null)
            {
                var list = new ArrayList();
                GetAttributesInternal(list, type);
                for (int i = 0; i < list.Count; i++)
                    if (attributeType.IsInstanceOfType(list[i]) == false)
                        list.RemoveAt(i--);
                _typeAttributes[key] = attrs = (object[]) list.ToArray(typeof (Attribute));
            }
            return attrs;
        }

        public static Attribute GetFirstAttribute(Type type, Type attributeType)
        {
            object[] attrs = new TypeHelper(type).GetAttributes(attributeType);
            return attrs.Length > 0 ? (Attribute) attrs[0] : null;
        }

        public static T GetFirstAttribute<T>(Type type) where T : Attribute
        {
            object[] attrs = new TypeHelper(type).GetAttributes(typeof (T));
            return attrs.Length > 0 ? (T) attrs[0] : null;
        }

        #region Attributes cache

        private static readonly Hashtable _typeAttributesTopInternal = new Hashtable(10);

        private static readonly Hashtable _typeAttributesInternal = new Hashtable(10);

        private static readonly Hashtable _typeAttributes = new Hashtable(10);

        private object[] GetAttributesInternal()
        {
            string key = _type.FullName;
            var attrs = (object[]) _typeAttributes[key];
            if (attrs == null)
            {
                var list = new ArrayList();
                GetAttributesInternal(list, _type);
                _typeAttributes[key] = attrs = (object[]) list.ToArray(typeof (Attribute));
            }
            return attrs;
        }

        private static void GetAttributesInternal(ArrayList list, Type type)
        {
            var attrs = (object[]) _typeAttributesTopInternal[type];
            if (attrs != null)
            {
                list.AddRange(attrs);
            }
            else
            {
                GetAttributesTreeInternal(list, type);
                _typeAttributesTopInternal[type] = list.ToArray(typeof (Attribute));
            }
        }

        private static void GetAttributesTreeInternal(ArrayList list, Type type)
        {
            var attrs = (object[]) _typeAttributesInternal[type];
            if (attrs == null)
                _typeAttributesInternal[type] = attrs = type.GetCustomAttributes(false);

            list.AddRange(attrs);
            if (type.IsInterface == false)
            {
                Type[] interfaces = type.GetInterfaces();
                int nBaseInterfaces = type.BaseType != null ? type.BaseType.GetInterfaces().Length : 0;
                for (int i = 0; i < interfaces.Length; i++)
                {
                    Type intf = interfaces[i];
                    if (i < nBaseInterfaces)
                    {
                        bool getAttr = false;
                        foreach (MethodInfo mi in type.GetInterfaceMap(intf).TargetMethods)
                        {
                            if (mi.DeclaringType == type)
                            {
                                getAttr = true;
                                break;
                            }
                        }
                        if (getAttr == false)
                            continue;
                    }
                    GetAttributesTreeInternal(list, intf);
                }
                if (type.BaseType != null && type.BaseType != typeof (object))
                    GetAttributesTreeInternal(list, type.BaseType);
            }
        }

        #endregion

        #endregion

        #region Property Wrappers

        public string FullName
        {
            get { return _type.FullName; }
        }

        public string Name
        {
            get { return _type.Name; }
        }

        public bool IsAbstract
        {
            get { return _type.IsAbstract; }
        }

        public bool IsArray
        {
            get { return _type.IsArray; }
        }

        public bool IsValueType
        {
            get { return _type.IsValueType; }
        }

        public bool IsClass
        {
            get { return _type.IsClass; }
        }

        public bool IsInterface
        {
            get { return _type.IsInterface; }
        }

        public bool IsSerializable
        {
            get { return _type.IsSerializable; }
        }

        #endregion

        #region GetMethods

        public MethodInfo[] GetMethods()
        {
            return _type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public MethodInfo[] GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
        }

        public MethodInfo[] GetMethods(BindingFlags flags)
        {
            return _type.GetMethods(flags);
        }

        public MethodInfo[] GetMethods(bool generic)
        {
            return GetMethods(_type, generic, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public MethodInfo[] GetPublicMethods(bool generic)
        {
            return GetMethods(_type, generic, BindingFlags.Instance | BindingFlags.Public);
        }

        public MethodInfo[] GetMethods(bool generic, BindingFlags flags)
        {
            return GetMethods(_type, generic, flags);
        }

        #endregion

        #region GetMethod

        public MethodInfo GetMethod(string methodName)
        {
            return _type.GetMethod(methodName,
                                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public MethodInfo GetPublicMethod(string methodName)
        {
            return _type.GetMethod(methodName,
                                   BindingFlags.Instance | BindingFlags.Public);
        }

        public MethodInfo GetMethod(string methodName, BindingFlags flags)
        {
            return _type.GetMethod(methodName, flags);
        }

        public MethodInfo GetPublicMethod(string methodName, params Type[] types)
        {
            return _type.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public,
                null,
                types,
                null);
        }

        public MethodInfo GetMethod(string methodName, params Type[] types)
        {
            return _type.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                types,
                null);
        }

        public MethodInfo GetMethod(string methodName, BindingFlags flags, params Type[] types)
        {
            return _type.GetMethod(methodName, flags, null, types, null);
        }

        public MethodInfo GetMethod(bool generic, string methodName)
        {
            return GetMethod(_type, generic, methodName,
                             BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public MethodInfo GetPublicMethod(bool generic, string methodName)
        {
            return GetMethod(_type, generic, methodName,
                             BindingFlags.Instance | BindingFlags.Public);
        }

        public MethodInfo GetMethod(bool generic, string methodName, BindingFlags flags)
        {
            return GetMethod(_type, generic, methodName, flags);
        }

        public MethodInfo GetPublicMethod(bool generic, string methodName, params Type[] types)
        {
            return _type.GetMethod(methodName,
                                   BindingFlags.Instance | BindingFlags.Public,
                                   generic ? GenericBinder.Generic : GenericBinder.NonGeneric,
                                   types, null);
        }

        public MethodInfo GetMethod(bool generic, string methodName, params Type[] types)
        {
            return _type.GetMethod(methodName,
                                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                   generic ? GenericBinder.Generic : GenericBinder.NonGeneric,
                                   types, null);
        }

        public MethodInfo GetMethod(bool generic, string methodName, BindingFlags flags, params Type[] types)
        {
            return _type.GetMethod(methodName,
                                   flags,
                                   generic ? GenericBinder.Generic : GenericBinder.NonGeneric,
                                   types, null);
        }

        #endregion

        #region GetFields

        public FieldInfo[] GetFields()
        {
            return _type.GetFields();
        }

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return _type.GetFields(bindingFlags);
        }

        public FieldInfo GetField(string name)
        {
            return _type.GetField(
                name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        #endregion

        #region GetProperties

        public PropertyInfo[] GetProperties()
        {
            return _type.GetProperties();
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
            return _type.GetProperties(bindingFlags);
        }

        public PropertyInfo GetProperty(string name)
        {
            return _type.GetProperty(
                name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        #endregion

        #region GetInterfaces

        public InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            return _type.GetInterfaceMap(interfaceType);
        }

        #endregion

        #region GetConstructor

        public ConstructorInfo GetPublicConstructor(params Type[] types)
        {
            return _type.GetConstructor(types);
        }

        public ConstructorInfo GetConstructor(Type parameterType)
        {
            return GetConstructor(_type, parameterType);
        }

        public static ConstructorInfo GetConstructor(Type type, params Type[] types)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                types,
                null);
        }

        public ConstructorInfo GetPublicDefaultConstructor()
        {
            return _type.GetConstructor(Type.EmptyTypes);
        }

        public ConstructorInfo GetDefaultConstructor()
        {
            return GetDefaultConstructor(_type);
        }

        public static ConstructorInfo GetDefaultConstructor(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
        }

        public ConstructorInfo[] GetPublicConstructors()
        {
            return _type.GetConstructors();
        }

        public ConstructorInfo[] GetConstructors()
        {
            return _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        #endregion
    }
}