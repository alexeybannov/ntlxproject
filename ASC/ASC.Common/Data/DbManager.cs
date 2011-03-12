using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using ASC.Common.Data.AdoProxy;
using ASC.Common.Data.Sql;
using ASC.Common.Web;
using log4net;

namespace ASC.Common.Data
{
    public class DbManager : IDisposable
    {
        private readonly ILog logger = LogManager.GetLogger("ASC.SQL");
        private readonly ProxyCtx proxyContext;
        private readonly bool shared;

        private IDbCommand command;
        private ISqlDialect dialect;


        public DbManager(string databaseId)
            : this(databaseId, true)
        {
        }

        public DbManager(string databaseId, bool shared)
        {
            if (databaseId == null) throw new ArgumentNullException("databaseId");
            DatabaseId = databaseId;
            this.shared = shared;

            if (logger.IsDebugEnabled)
            {
                proxyContext = new ProxyCtx();
                proxyContext.ExecutedEvent += AdoProxyExecutedEventHandler;
            }
        }

        private IDbCommand Command
        {
            get
            {
                CheckDispose();
                if (command == null) command = OpenConnection().CreateCommand();

                if (command.Connection.State == ConnectionState.Closed ||
                    command.Connection.State == ConnectionState.Broken)
                {
                    command = OpenConnection().CreateCommand();
                }
                return command;
            }
        }

        public string DatabaseId { get; private set; }

        public bool InTransaction
        {
            get { return Command.Transaction != null; }
        }

        public IDbConnection Connection
        {
            get { return Command.Connection; }
        }

        public bool Disposed { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                if (Disposed || (shared && HttpContext.Current != null)) return;
                Disposed = true;
                if (command != null)
                {
                    if (command.Connection != null) command.Connection.Dispose();
                    command.Dispose();
                    command = null;
                }
            }
        }

        #endregion

        public static DbManager FromHttpContext(string databaseId)
        {
            if (HttpContext.Current != null)
            {
                var dbManager = DisposableHttpContext.Current[databaseId] as DbManager;
                if (dbManager == null || dbManager.Disposed)
                {
                    dbManager = new DbManager(databaseId);
                    DisposableHttpContext.Current[databaseId] = dbManager;
                }
                return dbManager;
            }
            return new DbManager(databaseId);
        }

        public IDbConnection OpenConnection()
        {
            CheckDispose();
            IDbConnection connection = null;
            string key = null;
            if (shared && HttpContext.Current != null)
            {
                key = string.Format("Connection {0}|{1}", GetDialect(), DbRegistry.GetConnectionString(DatabaseId));
                connection = DisposableHttpContext.Current[key] as IDbConnection;
                if (connection != null) return connection;
            }
            connection = DbRegistry.CreateDbConnection(DatabaseId);
            if (proxyContext != null)
            {
                connection = new DbConnectionProxy(connection, proxyContext);
            }
            connection.Open();
            if (shared && HttpContext.Current != null) DisposableHttpContext.Current[key] = connection;
            return connection;
        }

        public IDbTransaction BeginTransaction()
        {
            if (InTransaction) throw new InvalidOperationException("Transaction already open.");

            Command.Transaction = Command.Connection.BeginTransaction();

            var tx = new DbTransaction(Command.Transaction);
            tx.Unavailable += TransactionUnavailable;
            return tx;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (InTransaction) throw new InvalidOperationException("Transaction already open.");

            Command.Transaction = Command.Connection.BeginTransaction(il);

            var tx = new DbTransaction(Command.Transaction);
            tx.Unavailable += TransactionUnavailable;
            return tx;
        }

        public void CommitTransaction()
        {
            if (command != null && command.Transaction != null)
            {
                command.Transaction.Commit();
            }
        }

        public List<object[]> ExecuteList(string sql)
        {
            return ExecuteList(sql, null);
        }

        public List<object[]> ExecuteList(string sql, params object[] parameters)
        {
            return Command.ExecuteList(sql, parameters);
        }

        public List<object[]> ExecuteListParam(string sql, Dictionary<string, object> parameters)
        {
            return Command.ExecuteListParam(sql, parameters);
        }

        public List<object[]> ExecuteList(ISqlInstruction sql)
        {
            return Command.ExecuteList(sql, GetDialect());
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql) where T : new()
        {
            return Command.ExecuteList<T>(sql, GetDialect());
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<IDataRecord, T> converter)
        {
            return Command.ExecuteList(sql, GetDialect(), converter);
        }

        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql);
        }

        public T ExecuteScalar<T>(string sql)
        {
            return ExecuteScalar<T>(sql, null);
        }

        public object ExecuteScalar(string sql, params object[] parameters)
        {
            return Command.ExecuteScalar(sql, parameters);
        }

        public T ExecuteScalar<T>(string sql, params object[] parameters)
        {
            return Command.ExecuteScalar<T>(sql, parameters);
        }

        public T ExecuteScalar<T>(ISqlInstruction sql)
        {
            return Command.ExecuteScalar<T>(sql, GetDialect());
        }

        public object ExecuteScalar(ISqlInstruction sql)
        {
            return Command.ExecuteScalar(sql, GetDialect());
        }

        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            return Command.ExecuteNonQuery(sql, parameters);
        }

        public int ExecuteNonQuery(ISqlInstruction sql)
        {
            return Command.ExecuteNonQuery(sql, GetDialect());
        }

        public int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            if (batch == null) throw new ArgumentNullException("batch");

            var affected = 0;
            using (var tx = BeginTransaction())
            {
                foreach (var sql in batch)
                {
                    affected += ExecuteNonQuery(sql);
                }
                tx.Commit();
            }
            return affected;
        }

        private void TransactionUnavailable(object sender, EventArgs e)
        {
            if (Command.Transaction != null)
            {
                Command.Transaction = null;
            }
        }

        private void CheckDispose()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        private ISqlDialect GetDialect()
        {
            if (dialect == null) dialect = DbRegistry.GetSqlDialect(DatabaseId);
            return dialect;
        }

        private void AdoProxyExecutedEventHandler(object sender, ExecutedEventArgs a)
        {
            //remove linebrakes and tabs for log analyzing
            var desc = a.Description.Replace(Environment.NewLine, " ").Replace("\n", "").Replace("\r", "").Replace("\t", " ");
            logger.DebugFormat("({0:####} ms) {1}.{2}", (int)Math.Ceiling(a.Duration.TotalMilliseconds), a.Executed, desc);
        }
    }
}