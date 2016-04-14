using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Debonair.Data.Context
{
    public interface IContext : IDisposable
    {
        SqlConnection SqlConnection { get; set; }
        void Open();
        void Close();

        void ExecuteNonQuery<T>(string sql, List<SqlParameter> parameters);
        void ExecuteNonQuery<T>(string sql);

        int ExecuteScalar<T>(string sql, List<SqlParameter> parameters);
        int ExecuteScalar<T>(string sql);

        IEnumerable<T> Query<T>(string sql, List<SqlParameter> parameters);
        IEnumerable<T> Query<T>(string sql);
    }
}
