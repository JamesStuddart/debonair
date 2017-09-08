using System;
using System.Collections.Generic;
using System.Data;

namespace Debonair.Data.Context
{
    public interface IContext : IDisposable
    {
        IDbConnection dbConnection { get; set; }
        void Open();
        void Close();

        void ExecuteNonQuery(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text);
        void ExecuteNonQuery(string sql);

        object ExecuteScalar<TEntity>(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new();
        object ExecuteScalar<TEntity>(string sql) where TEntity : class, new();

        IEnumerable<TEntity> Query<TEntity>(string sql, List<IDbDataParameter> parameters, CommandType commandType = CommandType.Text) where TEntity : class, new();
        IEnumerable<TEntity> Query<TEntity>(string sql, CommandType commandType = CommandType.Text) where TEntity : class, new();

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName, List<IDbDataParameter> parameters) where TEntity : class, new();
        void ExecuteStoredProcedure(string spName, List<IDbDataParameter> parameters);
        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string spName) where TEntity : class, new();
        void ExecuteStoredProcedure(string spName);
    }
}
