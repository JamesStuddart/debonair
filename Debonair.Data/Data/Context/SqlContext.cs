using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Debonair.Utilities.Extensions;


namespace Debonair.Data.Context
{
    public class SqlContext : IContext
    {
        private bool disposed;

        public SqlConnection SqlConnection { get; set; }

        public SqlContext(SqlConnection connection)
        {
            SqlConnection = connection;
        }

        ~SqlContext()
        {
            Dispose(false);
        }

        public void Open()
        {
            if (SqlConnection.State == ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }

        public void Close()
        {
            SqlConnection.Close();
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
                SqlConnection.Close();
            }

            disposed = true;
        }

        #region nonquery
        public void ExecuteNonQuery(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text)
        {
            var cmd = SqlConnection.CreateCommand();
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
            ExecuteNonQuery(sql, new List<SqlParameter>());
        }
        #endregion nonquery

        #region ExecuteScalar
        public int ExecuteScalar<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text)
        {
            var cmd = SqlConnection.CreateCommand();
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
        
        public int ExecuteScalar<TEntity>(string sql)
        {
            return ExecuteScalar<TEntity>(sql, new List<SqlParameter>());
        }
        #endregion ExecuteScalar

        #region query

        public IEnumerable<TEntity> Query<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text)
        {
            var cmd = SqlConnection.CreateCommand();
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

        public IEnumerable<TEntity> Query<TEntity>(string sql, SqlParameter parameter)
        {
            return Query<TEntity>(sql, new List<SqlParameter> { parameter });
        }

        public IEnumerable<TEntity> Query<TEntity>(string sql, CommandType commandType = CommandType.Text)
        {
            return Query<TEntity>(sql, new List<SqlParameter>());
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, List<SqlParameter> parameters)
        {
            return Query<TEntity>(spName, parameters, CommandType.StoredProcedure);

        }

        public void ExecuteStoredProcedure(string spName, List<SqlParameter> parameters)
        {
             ExecuteNonQuery(spName, parameters, CommandType.StoredProcedure);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName)
        {
            return Query<TEntity>(spName, new List<SqlParameter>(), CommandType.StoredProcedure);
        }

        public void ExecuteStoredProcedure(string spName)
        {
             ExecuteNonQuery(spName, new List<SqlParameter>(), CommandType.StoredProcedure);
        }

        #endregion query
    }
}
