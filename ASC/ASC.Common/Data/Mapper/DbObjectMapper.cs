#region usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ASC.Common.Data.Sql;

#endregion

namespace ASC.Common.Data.Mapper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public DbColumnAttribute(string column, bool isKey, bool isIdentity)
            : this(column, isKey, isIdentity, null)
        {
        }

        public DbColumnAttribute(string column)
            : this(column, null)
        {
        }

        public DbColumnAttribute(string column, Type dataBaseType)
            : this(column, false, dataBaseType)
        {
        }

        public DbColumnAttribute(string column, bool isKey, Type dataBaseType)
            : this(column, isKey, false, dataBaseType)
        {
        }

        public DbColumnAttribute(string column, bool isKey, bool isIdentity, Type dataBaseType)
        {
            IsKey = isKey;
            IsIdentity = isIdentity;
            DataBaseType = dataBaseType;
            Column = column;
        }

        public bool IsKey { get; private set; }
        public bool IsIdentity { get; private set; }
        public Type DataBaseType { get; private set; }
        public string Column { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute : Attribute
    {
        public DbTableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NoDbAttribute : Attribute
    {
    }

    public class DbObjectMapper<T> where T : new()
    {
        private readonly Dictionary<string, PropertyInfo> _bindColumn = new Dictionary<string, PropertyInfo>();
        private readonly PropertyInfo _identity;
        private readonly List<PropertyInfo> _keys = new List<PropertyInfo>();

        private readonly Dictionary<DbColumnAttribute, PropertyInfo> _mappedColumn =
            new Dictionary<DbColumnAttribute, PropertyInfo>();

        public DbObjectMapper()
        {
            object[] tableAttr = typeof (T).GetCustomAttributes(typeof (DbTableAttribute), true);
            if (tableAttr.Length > 0)
            {
                Table = (DbTableAttribute) tableAttr[0];
            }
            IEnumerable<PropertyInfo> properties =
                typeof (T).GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).Where(
                    x => x.GetCustomAttributes(typeof (NoDbAttribute), true).Length == 0);
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] nodataAttr = propertyInfo.GetCustomAttributes(typeof (NoDbAttribute), true);
                if (nodataAttr.Length == 0)
                {
                    object[] dataAttr = propertyInfo.GetCustomAttributes(typeof (DbColumnAttribute), true);
                    if (dataAttr.Length > 0)
                    {
                        var dataColumn = (DbColumnAttribute) dataAttr[0];
                        _bindColumn.Add(dataColumn.Column, propertyInfo);
                        _mappedColumn.Add(dataColumn, propertyInfo);
                        if (dataColumn.IsIdentity)
                        {
                            _identity = propertyInfo;
                        }
                        if (dataColumn.IsKey)
                        {
                            _keys.Add(propertyInfo);
                        }
                    }
                    else
                    {
                        _bindColumn.Add(propertyInfo.Name, propertyInfo);
                        _bindColumn.Add(propertyInfo.Name.ToLowerInvariant(), propertyInfo);
                    }
                }
            }
        }

        protected DbTableAttribute Table { get; private set; }

        public string TableName
        {
            get
            {
                if (Table != null)
                {
                    return Table.TableName;
                }
                return string.Empty;
            }
        }

        public bool HasKey
        {
            get { return _keys.Count > 0; }
        }

        public bool HasIdentity
        {
            get { return _identity != null; }
        }

        public T Map(IDataRecord input)
        {
            var mapped = new T();
            for (int i = 0; i < input.FieldCount; i++)
            {
                if (!input.IsDBNull(i))
                {
                    string fieldName = input.GetName(i);
                    Type fieldType = input.GetFieldType(i);

                    PropertyInfo propertyToMap = null;
                    if (_bindColumn.ContainsKey(fieldName))
                    {
                        propertyToMap = _bindColumn[fieldName];
                    }
                    else if (_bindColumn.ContainsKey(fieldName.ToLowerInvariant()))
                    {
                        propertyToMap = _bindColumn[fieldName.ToLowerInvariant()];
                    }
                    if (propertyToMap != null)
                    {
                        Type propertyType = propertyToMap.PropertyType;
                        if (propertyType == fieldType)
                        {
                            propertyToMap.SetValue(mapped, input.GetValue(i), null);
                        }
                        else
                        {
                            try
                            {
                                object propertyConvertedValue = Convert.ChangeType(input.GetValue(i), propertyType);
                                if (propertyConvertedValue != null)
                                {
                                    propertyToMap.SetValue(mapped, propertyConvertedValue, null);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            return mapped;
        }

        public void SetIdentity(T item, object identity)
        {
            if (HasIdentity && identity != null)
            {
                if (_identity.PropertyType != identity.GetType())
                {
                    identity = Convert.ChangeType(identity, _identity.PropertyType);
                    if (identity != null)
                    {
                        _identity.SetValue(item, identity, null);
                    }
                }
            }
        }

        public void InvokeAction(IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> props,
                                 Action<string, object> action, T item)
        {
            if (action != null)
            {
                foreach (var info in props)
                {
                    string propColumnName = info.Key.Column;

                    object propValue = info.Value.GetValue(item, null);
                    if (info.Key.DataBaseType != null)
                    {
                        propValue = Convert.ChangeType(propValue, info.Key.DataBaseType);
                    }
                    action(propColumnName, propValue);
                }
            }
        }

        public void InvokeAction(IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> props,
                                 Action<string, object> action, params object[] keys)
        {
            if (action != null)
            {
                if (keys == null) throw new ArgumentNullException("keys");

                IEnumerable<Type> types = keys.Select(x => x.GetType());
                List<KeyValuePair<DbColumnAttribute, PropertyInfo>> keyed =
                    props.Where(x => x.Key.IsKey || x.Key.IsIdentity).Where(
                        x => types.Contains(x.Value.PropertyType)).ToList();
                foreach (object key in keys)
                {
                    object key1 = key;
                    KeyValuePair<DbColumnAttribute, PropertyInfo> propDescriptor =
                        keyed.Where(x => x.Value.PropertyType == key1.GetType()).Single();
                    action(propDescriptor.Key.Column, key1);
                }
            }
        }

        public void InsertCommand(SqlInsert slqInsert, T item)
        {
            InvokeAction(_mappedColumn, (name, value) => slqInsert.InColumnValue(name, value), item);
        }

        public void Where(SqlQuery sql, T item)
        {
            IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> keyed =
                _mappedColumn.Where(x => x.Key.IsKey || x.Key.IsIdentity);
            InvokeAction(keyed, (name, value) => sql.Where(name, value), item);
        }

        public void Where(SqlDelete sql, T item)
        {
            IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> keyed =
                _mappedColumn.Where(x => x.Key.IsKey || x.Key.IsIdentity);
            InvokeAction(keyed, (name, value) => sql.Where(name, value), item);
        }

        public void Where(SqlUpdate sql, T item)
        {
            IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> keyed =
                _mappedColumn.Where(x => x.Key.IsKey || x.Key.IsIdentity);
            InvokeAction(keyed, (name, value) => sql.Where(name, value), item);
        }

        public void Where(SqlQuery sql, params object[] keys)
        {
            InvokeAction(_mappedColumn, (name, value) => sql.Where(name, value), keys);
        }

        public void Where(SqlDelete sql, params object[] keys)
        {
            InvokeAction(_mappedColumn, (name, value) => sql.Where(name, value), keys);
        }

        public bool DbEqual(T item, T other)
        {
            IEnumerable<KeyValuePair<DbColumnAttribute, PropertyInfo>> keyed =
                _mappedColumn.Where(x => x.Key.IsKey || x.Key.IsIdentity);
            return !(from info in keyed
                     let itemValue = info.Value.GetValue(item, null)
                     let otherValue = info.Value.GetValue(other, null)
                     where !Equals(itemValue, otherValue)
                     select itemValue).Any();
        }

        public void UpdateCommand(SqlUpdate sqlUpdate, T item)
        {
            InvokeAction(_mappedColumn.Where(x => !x.Key.IsIdentity), (name, value) => sqlUpdate.Set(name, value), item);
        }
    }
}