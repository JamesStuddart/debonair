using System;
using System.Collections.Generic;
using System.Data;
using Debonair.Utilities.Extensions;


namespace Debonair.Data.Context
{
    public class SqlContext : IContext
    {
        private bool disposed;

        public IDbConnection dbConnection { get; set; }

        public SqlContext(IDbConnection connection)
        {
            dbConnection = connection;
        }

        ~SqlContext()
        {
            Dispose(false);
        }

        public void Open()
        {
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }
        }

        public void Close()
        {
            dbConnection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if (disposed) return;

            if (dispose)
            {
                dbConnection.Close();
            }

            disposed = true;
        }

        #region nonquery
        public void ExecuteNonQuery(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text)
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            try
            {
                Open();

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Close();
            }

        }

        public void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, new List<IDbDataParameter>());
        }
        #endregion nonquery

        #region ExecuteScalar
        public int ExecuteScalar<TEntity>(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new()
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            int result;

            try
            {
                Open();

                result = (int)cmd.ExecuteScalar();
            }
            finally
            {
                Close();
            }

            return result;
        }
        
        public int ExecuteScalar<TEntity>(string sql) where TEntity : class, new()
        {
            return ExecuteScalar<TEntity>(sql, new List<IDbDataParameter>());
        }
        #endregion ExecuteScalar

        #region query

        public IEnumerable<TEntity> Query<TEntity>(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new()
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = sql;

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            IEnumerable<TEntity> result;

            try
            {
                Open();

                result = cmd.ExecuteReader().MapTo<TEntity>();
            }
            finally
            {
                Close();
            }

            return result;
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, IDbDataParameter parameter) where TEntity : class, new()
        {
            return Query<TEntity>(sql, new List<IDbDataParameter> { parameter });
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, CommandType commandType = CommandType.Text) where TEntity : class, new()
        {
            return Query<TEntity>(sql, new List<IDbDataParameter>());
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, List<IDbDataParameter> parameters) where TEntity : class, new()
        {
            return Query<TEntity>(spName, parameters, CommandType.StoredProcedure);

        }

        public void ExecuteStoredProcedure(string spName, List<IDbDataParameter> parameters)
        {
             ExecuteNonQuery(spName, parameters, CommandType.StoredProcedure);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName) where TEntity : class, new()
        {
            return Query<TEntity>(spName, new List<IDbDataParameter>(), CommandType.StoredProcedure);
        }

        public void ExecuteStoredProcedure(string spName)
        {
             ExecuteNonQuery(spName, new List<IDbDataParameter>(), CommandType.StoredProcedure);
        }

        #endregion query
    }
}
