using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Debonair.Data.Context
{
    public interface IContext : IDisposable
    {
        SqlConnection SqlConnection { get; set; }
        void Open();
        void Close();

        void ExecuteNonQuery(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text);
        void ExecuteNonQuery(string sql);

        int ExecuteScalar<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new();
        int ExecuteScalar<TEntity>(string sql) where TEntity : class, new();

        IEnumerable<TEntity> Query<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new();
        IEnumerable<TEntity> Query<TEntity>(string sql, CommandType commandType = CommandType.Text) where TEntity : class, new();

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, List<SqlParameter> parameters) where TEntity : class, new();
        void ExecuteStoredProcedure(string spName, List<SqlParameter> parameters);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName) where TEntity : class, new();
        void ExecuteStoredProcedure(string spName);
    }
}
