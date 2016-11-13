using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Debonair.Data.Orm;
using Debonair.Data.Context;
using Debonair.FluentApi;
using Debonair.Utilities;

namespace Debonair.Data
{
    public class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class, new()
    {
        private bool disposed;

        private readonly ICrudGenerator<TEntity> sqlGenerator;
        private readonly IContext dataContext;


        public DataRepository(SqlConnection sqlConnection, ICrudGenerator<TEntity> generator = null, IContext context = null)
        {
            sqlGenerator = generator ?? new SqlCrudGenerator<TEntity>();
            dataContext = context ?? new SqlContext(sqlConnection);
        }
        ~DataRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (disposed) return;

            if (dispose)
            {
                dataContext.Dispose();
            }

            disposed = true;
        }


        public IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate = null, bool dirtyRead = true)
        {
            var sql = sqlGenerator.Select(predicate, dirtyRead);
            return dataContext.Query<TEntity>(sql, sqlGenerator.SelectParameters.ToSqlParameters());
        }

        public bool Insert(TEntity entity)
        {
            var sql = sqlGenerator.Insert();
            var newId = dataContext.ExecuteScalar<TEntity>(sql, entity.ToSqlParameters<TEntity>());

            var mapping = EntityMappingEngine.GetMappingForEntity<TEntity>();

            var propMapping = mapping?.PropertyMappings.FirstOrDefault(x => x.IsPrimaryKey);

            propMapping?.PropertyInfo.SetValue(entity, Convert.ChangeType(newId, propMapping.PropertyInfo.PropertyType), null);

            return true;

        }

        public bool Update(TEntity entity)
        {
            var sql = sqlGenerator.Update();
            dataContext.ExecuteNonQuery(sql, entity.ToSqlParameters<TEntity>());

            return true;
        }

        public bool Delete(TEntity entity, bool forceDelete = false)
        {
            var sql = sqlGenerator.Delete();
            dataContext.ExecuteNonQuery(sql);

            return true;
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure(string spName, object parameters)
        {
            return dataContext.ExecuteStoredProcedure<TEntity>(spName, parameters.ToSqlParameters<TEntity>());
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure(string spName)
        {
            return dataContext.ExecuteStoredProcedure<TEntity>(spName);
        }


        public void ExecuteNonQueryStoredProcedure(string spName, object parameters)
        {
            dataContext.ExecuteStoredProcedure(spName, parameters.ToSqlParameters<TEntity>());
        }

        public void ExecuteNonQueryStoredProcedure(string spName)
        {
            dataContext.ExecuteStoredProcedure(spName);
        }
    }
}
