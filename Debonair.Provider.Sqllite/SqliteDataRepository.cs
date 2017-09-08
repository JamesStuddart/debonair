using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Debonair.Data;
using Debonair.FluentApi;
using Debonair.Provider.Sqlite.Data.Context;
using Debonair.Utilities;

namespace Debonair.Provider.Sqlite
{
    public class SqliteDataRepository<TEntity> : IDataRepository<TEntity> where TEntity : class, new()
    {
        private bool _disposed;
        private readonly IDataProvider<TEntity> _dataProvider;

        public SqliteDataRepository(SqliteProvider<TEntity> dataProvider)
        {
            _dataProvider = dataProvider;
        }

        ~SqliteDataRepository()
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
            if (_disposed) return;

            if (dispose)
            {
                _dataProvider.DataContext.Dispose();
            }

            _disposed = true;
        }

        public IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> predicate = null)
        {
            var sql = _dataProvider.CrudGenerator.Select(predicate); 
            return _dataProvider.DataContext.Query<TEntity>(sql, _dataProvider.CrudGenerator.SelectParameters.ToDbDataParameters(_dataProvider.DbConnection));
        }

        public bool Insert(TEntity entity)
        {
            var sql = _dataProvider.CrudGenerator.Insert();
            var newId = _dataProvider.DataContext.ExecuteScalar<TEntity>(sql, entity.ToDbDataParameters<TEntity>(_dataProvider.DbConnection));

            var mapping = MappingCache.GetMappingForEntity<TEntity>();

            var propMapping = mapping?.PropertyMappings.FirstOrDefault(x => x.IsPrimaryKey);

            propMapping?.PropertyInfo.SetValue(entity, Convert.ChangeType(newId, propMapping.PropertyInfo.PropertyType), null);

            return true;
        }

        public bool Update(TEntity entity)
        {
            var sql = _dataProvider.CrudGenerator.Update();
            _dataProvider.DataContext.ExecuteNonQuery(sql, entity.ToDbDataParameters<TEntity>(_dataProvider.DbConnection));

            return true;
        }

        public bool Delete(TEntity entity, bool forceDelete = false)
        {
            var sql = _dataProvider.CrudGenerator.Delete();
            _dataProvider.DataContext.ExecuteNonQuery(sql);

            return true;
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure(string spName, object parameters)
        {
            return _dataProvider.DataContext.ExecuteStoredProcedure<TEntity>(spName, parameters.ToDbDataParameters<TEntity>(_dataProvider.DbConnection));
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure(string spName)
        {
            return _dataProvider.DataContext.ExecuteStoredProcedure<TEntity>(spName);
        }

        public void ExecuteNonQueryStoredProcedure(string spName, object parameters)
        {
            _dataProvider.DataContext.ExecuteStoredProcedure(spName, parameters.ToDbDataParameters<TEntity>(_dataProvider.DbConnection));
        }

        public void ExecuteNonQueryStoredProcedure(string spName)
        {
            _dataProvider.DataContext.ExecuteStoredProcedure(spName);
        }
    }
}
