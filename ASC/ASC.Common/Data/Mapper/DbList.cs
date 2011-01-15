#region usings

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ASC.Common.Data.Mapper.Sql;
using ASC.Common.Data.Sql;

#endregion

namespace ASC.Common.Data.Mapper
{
    internal class DbList<T> : ICollection<T> where T : new()
    {
        private readonly string dbId;
        private readonly DbObjectMapper<T> mapper;
        private List<T> innerList;

        public DbList(string dbid)
        {
            dbId = dbid;
            mapper = new DbObjectMapper<T>();
            Table = mapper.TableName;
        }

        public string Table { get; private set; }

        #region ICollection<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            PreloadList();
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sqlInsert = new SqlInsert(Table, false);
                if (mapper.HasIdentity)
                {
                    sqlInsert.Identity(0, 0, true);
                }
                mapper.InsertCommand(sqlInsert, item);
                object identity = db.ExecuteScalar(sqlInsert);
                mapper.SetIdentity(item, identity);
                tx.Commit();
                if (innerList != null)
                {
                    innerList.Add(item);
                }
            }
        }

        public void Clear()
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sql = new SqlDelete(Table);
                db.ExecuteNonQuery(sql);
                tx.Commit();
                if (innerList != null)
                {
                    innerList.Clear();
                }
            }
        }

        public bool Contains(T item)
        {
            var sql = new SqlQuery(Table);
            mapper.Where(sql, item);
            sql.SelectCount();
            using (DbManager db = GetDbManager())
            {
                return db.ExecuteScalar<int>(sql) > 0;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            PreloadList();
            innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sql = new SqlDelete(Table);
                mapper.Where(sql, item);
                int modified = db.ExecuteNonQuery(sql);
                tx.Commit();
                if (innerList != null)
                {
                    T exitsing = innerList.Where(x => mapper.DbEqual(item, x)).SingleOrDefault();
                    innerList.Remove(exitsing);
                }
                return modified > 0;
            }
        }

        public int Count
        {
            get
            {
                var sql = new SqlQuery(Table);
                sql.SelectCount();
                using (DbManager db = GetDbManager())
                {
                    return db.ExecuteScalar<int>(sql);
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        private void PreloadList()
        {
            if (innerList == null)
            {
                SqlQuery query = new SqlQuery(Table).SelectAll();
                using (DbManager db = GetDbManager())
                {
                    innerList = db.ExecuteList<T>(query, mapper.Map);
                }
            }
        }

        public T Find(params object[] keys)
        {
            using (DbManager db = GetDbManager())
            {
                return db.ExecuteList<T>(GetWhere(keys)).SingleOrDefault();
            }
        }

        public IEnumerable<T> Execute(SqlQuery query)
        {
            using (DbManager db = GetDbManager())
            {
                return db.ExecuteList<T>(query);
            }
        }

        public long ExecuteCount(SqlQuery query)
        {
            query.SelectCount().SetFirstResult(0).SetMaxResults(0);
            using (DbManager db = GetDbManager())
            {
                return db.ExecuteScalar<long>(query);
            }
        }

        public IEnumerable<T> Range(int from, int count, params object[] keys)
        {
            SqlQuery sql = GetWhere(keys).SetFirstResult(from).SetMaxResults(count);
            using (DbManager db = GetDbManager())
            {
                return db.ExecuteList<T>(sql);
            }
        }

        public SqlQuery GetWhere(params object[] keys)
        {
            return GetWhere(0, int.MaxValue, keys);
        }

        public DbSqlQuery<T> GetWhere(int from, int count, params object[] keys)
        {
            var sql = new DbSqlQuery<T>(this);
            mapper.Where(sql, keys);
            sql.SelectAll();
            if (from != -1 && count != int.MaxValue)
            {
                sql.SetFirstResult(from);
                sql.SetMaxResults(count);
            }
            return sql;
        }

        public bool SaveOrUpdate(T item)
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sqlInsert = new SqlInsert(Table, true);
                if (mapper.HasIdentity)
                {
                    sqlInsert.Identity(0, 0, true);
                }
                mapper.InsertCommand(sqlInsert, item);
                object identity = db.ExecuteScalar(sqlInsert);
                mapper.SetIdentity(item, identity);
                tx.Commit();
                if (innerList != null)
                {
                    innerList.Add(item);
                }
                return true;
            }
        }

        public bool Update(T item)
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sqlUpdate = new SqlUpdate(Table);
                mapper.Where(sqlUpdate, item);
                mapper.UpdateCommand(sqlUpdate, item);
                db.ExecuteNonQuery(sqlUpdate);
                tx.Commit();
                if (innerList != null)
                {
                    T exitsing = innerList.Where(x => mapper.DbEqual(item, x)).SingleOrDefault();
                    innerList.Remove(exitsing);
                    innerList.Add(item);
                }
                return true;
            }
        }

        public bool Remove(params object[] keys)
        {
            using (DbManager db = GetDbManager())
            using (IDbTransaction tx = db.BeginTransaction())
            {
                var sql = new SqlDelete(Table);
                mapper.Where(sql, keys);
                int modified = db.ExecuteNonQuery(sql);
                tx.Commit();
                return modified > 0;
            }
        }

        private DbManager GetDbManager()
        {
            return DbManager.FromHttpContext(dbId);
        }
    }
}