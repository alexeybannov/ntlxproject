#region usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ASC.Common.Data.Mapper;
using ASC.Common.Data.Sql;
using log4net;

#endregion

namespace ASC.Common.Data
{
    public static class DataExtensions
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.SQL");

        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText)
        {
            return CreateCommand(connection, commandText, null);
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText,
                                               params object[] parameters)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            command.AddParameters(parameters);
            return command;
        }

        public static List<object[]> ExecuteList(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                return command.ExecuteList(sql, parameters);
            }
        }

        public static T ExecuteScalar<T>(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                return command.ExecuteScalar<T>(sql, parameters);
            }
        }

        public static int ExecuteNonQuery(this IDbConnection connection, string sql, params object[] parameters)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                return command.ExecuteNonQuery(sql, parameters);
            }
        }

        public static IDbCommand SetParameters(this IDbCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return command;
            foreach (var param in parameters)
                AddParameter(command, param.Key, param.Value);
            return command;
        }

        public static IDbCommand AddParameter(this IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value != null ? value : DBNull.Value;
            command.Parameters.Add(parameter);
            return command;
        }

        public static IDbCommand AddNamedParameters(this IDbCommand command, Dictionary<string ,object > parameters)
        {
            if (parameters == null) return command;
            foreach (var kwp in parameters)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.Value = kwp.Value ?? DBNull.Value;
                parameter.ParameterName = kwp.Key;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public static IDbCommand AddParameters(this IDbCommand command, params object[] parameters)
        {
            if (parameters == null) return command;
            foreach (object value in parameters)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public static List<object[]> ExecuteList(this IDbCommand command)
        {
            return ExecuteList(command, command.CommandText, null);
        }

        public static List<object[]> ExecuteList(this IDbCommand command, string sql)
        {
            return ExecuteList(command, sql, null);
        }

        public static List<object[]> ExecuteListParam(this IDbCommand command, string sql, Dictionary<string,object> parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddNamedParameters(parameters);
            }
            return ExecuteListReader(command);
        }

        public static List<object[]> ExecuteList(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            return ExecuteListReader(command);
        }

        private static List<object[]> ExecuteListReader(IDbCommand command)
        {
            var list = new List<object[]>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new object[reader.FieldCount];
                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = reader[i];
                        if (DBNull.Value.Equals(row[i])) row[i] = null;
                    }
                    list.Add(row);
                }
            }
            return list;
        }

        public static T ExecuteScalar<T>(this IDbCommand command)
        {
            return ExecuteScalar<T>(command, command.CommandText, null);
        }

        public static T ExecuteScalar<T>(this IDbCommand command, string sql)
        {
            return ExecuteScalar<T>(command, sql, null);
        }

        public static object ExecuteScalar(this IDbCommand command)
        {
            return ExecuteScalar(command, command.CommandText);
        }

        public static object ExecuteScalar(this IDbCommand command, string sql)
        {
            return ExecuteScalar(command, sql, null);
        }

        public static object ExecuteScalar(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            object result = command.ExecuteScalar();
            return result;
        }

        public static T ExecuteScalar<T>(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            if (parameters != null)
            {
                command.Parameters.Clear();
                command.AddParameters(parameters);
            }
            object result = command.ExecuteScalar();
            return result == null || result == DBNull.Value ? default(T) : (T) Convert.ChangeType(result, typeof (T));
        }

        public static int ExecuteNonQuery(this IDbCommand command, string sql)
        {
            return ExecuteNonQuery(command, sql, null);
        }

        public static int ExecuteNonQuery(this IDbCommand command, string sql, params object[] parameters)
        {
            command.CommandText = sql;
            command.Parameters.Clear();
            command.AddParameters(parameters);
            int result = command.ExecuteNonQuery();
            return result;
        }

        public static List<object[]> ExecuteList(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteList();
        }

        public static List<T> ExecuteList<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
            where T : new()
        {
            var mapper = new DbObjectMapper<T>();
            return ExecuteList<T>(command, sql, dialect, mapper.Map);
        }

        public static List<T> ExecuteList<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect,
                                             Converter<IDataRecord, T> rowMapper)
        {
            ApplySqlInstruction(command, sql, dialect);
            var list = new List<T>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    list.Add(rowMapper(reader));
            }
            return list;
        }

        public static object ExecuteScalar(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteScalar();
        }

        public static T ExecuteScalar<T>(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteScalar<T>();
        }

        public static int ExecuteNonQuery(this IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            ApplySqlInstruction(command, sql, dialect);
            return command.ExecuteNonQuery();
        }

        private static void ApplySqlInstruction(IDbCommand command, ISqlInstruction sql, ISqlDialect dialect)
        {
            string sqlStr = sql.ToString(dialect);
            object[] parameters = sql.GetParameters();
            command.Parameters.Clear();
            if (dialect.SupportNotNamedParameters)
            {
                command.CommandText = sqlStr;
                command.AddParameters(parameters);
            }
            else
            {
                string[] sqlParts = sqlStr.Split('?');
                var sqlBuilder = new StringBuilder();
                for (int i = 0; i < sqlParts.Length - 1; i++)
                {
                    IDbDataParameter p = command.CreateParameter();
                    p.ParameterName = "p" + i;
                    p.Value = parameters[i];
                    command.Parameters.Add(p);
                    sqlBuilder.AppendFormat("{0}{1}{2}", sqlParts[i], dialect.ParameterSymbol, p.ParameterName);
                }
                sqlBuilder.Append(sqlParts[sqlParts.Length - 1]);
                command.CommandText = sqlBuilder.ToString();
            }
        }

        #region IDataRecord

        public static bool Exists(this IDataRecord rec, string name)
        {
            for (int i = 0; i < rec.FieldCount; i++)
                if (String.Compare(rec.GetName(i), name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
            return false;
        }

        public static T Get<T>(this IDataRecord rec, int index)
        {
            T result = default(T);
            if (index >= 0)
            {
                object val = rec.GetValue(index);
                if (val != DBNull.Value)
                {
                    if (typeof (T) == typeof (Guid))
                        val = rec.GetGuid(index);
                    result = (T) Convert.ChangeType(val, typeof (T));
                }
            }
            return result;
        }

        public static T Get<T>(this IDataRecord rec, string name)
        {
            if (!Exists(rec, name))
                return default(T);
            return Get<T>(rec, rec.GetOrdinal(name));
        }

        #endregion
    }
}