#region usings

using System.Collections.Generic;
using ASC.Common.Data.Mapper.Sql;
using ASC.Common.Data.Sql;

#endregion

namespace ASC.Common.Data.Mapper
{
    public abstract class DbActiveRecord<T> where T : class, new()
    {
        public static string DbId = string.Empty;
        private static DbList<T> list;

        private static DbList<T> List
        {
            get { return list ?? (list = new DbList<T>(DbId)); }
        }

        public void Create()
        {
            List.Add(this as T);
        }

        public bool Update()
        {
            return List.Update(this as T);
        }

        public bool SaveOrUpdate()
        {
            return List.SaveOrUpdate(this as T);
        }

        public bool Delete()
        {
            return List.Remove(this as T);
        }

        public static T Find(params object[] keys)
        {
            return List.Find(keys);
        }

        public static DbSqlQuery<T> Select(params object[] keys)
        {
            return Select(0, int.MaxValue, keys);
        }

        public static DbSqlQuery<T> Select(int from, int count, params object[] keys)
        {
            return List.GetWhere(from, count, keys);
        }

        public static IEnumerable<T> RunQuery(SqlQuery query)
        {
            return List.Execute(query);
        }

        public static IEnumerable<T> Range(int from, int count)
        {
            return List.Range(from, count);
        }

        public static int Count()
        {
            return List.Count;
        }
    }
}