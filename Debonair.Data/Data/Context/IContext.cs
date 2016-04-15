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

        int ExecuteScalar<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text);
        int ExecuteScalar<TEntity>(string sql);

        IEnumerable<TEntity> Query<TEntity>(string sql, List<SqlParameter> parameters, CommandType commandType = CommandType.Text);
        IEnumerable<TEntity> Query<TEntity>(string sql, CommandType commandType = CommandType.Text);

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, List<SqlParameter> parameters);
        void ExecuteStoredProcedure(string spName, List<SqlParameter> parameters);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName);
        void ExecuteStoredProcedure(string spName);
    }
}
