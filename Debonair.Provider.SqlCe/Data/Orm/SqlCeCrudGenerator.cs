using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Debonair.Data.Orm;
using Debonair.Data.Orm.QueryBuilder;
using Debonair.FluentApi;
using Debonair.Provider.SqlCe.Data.Context;
using Debonair.Utilities;

namespace Debonair.Provider.SqlCe.Data.Orm
{
    public class SqlCeCrudGenerator<TEntity> : ICrudGenerator<TEntity> where TEntity : class, new()
    {
        public Dictionary<string, object> SelectParameters { get; set; }
        public IEntityMapping<TEntity> EntityMapping { get; }
        
        public SqlCeCrudGenerator()
        {
            EntityMapping = MappingCache.GetMappingForEntity<TEntity>();
        }

        #region Query generators
        
        public virtual string Insert()
        {

            if (typeof(IEnumerable).IsAssignableFrom(typeof(TEntity)) && !typeof(string).IsAssignableFrom(typeof(TEntity)))
            {
                throw new NotSupportedException("To insert multiple lines of data it is faster and more effcient to use SQLBulkCopy ");
            }
            

            var columNames = string.Join(", ", EntityMapping.Properties.Where(x => !x.IsIgnored).Select(p => !string.IsNullOrEmpty(p.ColumnName) ? $"{EntityMapping.TableName}.{p.ColumnName}" : $"{EntityMapping.TableName}.{p.PropertyInfo.Name}"));
            var values = string.Join(", ", EntityMapping.Properties.Where(x => !x.IsIgnored).Select(p => $"@{p.PropertyInfo.Name}"));

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO {0} {1} {2} ",
                                    EntityMapping.TableName,
                                    string.IsNullOrEmpty(columNames) ? string.Empty : $"({columNames})",
                                    string.IsNullOrEmpty(values) ? string.Empty : $" VALUES ({values})");

            if (EntityMapping.PrimaryKey != null)
            {
                strBuilder.AppendLine("SELECT @@Identity");
            }

            return strBuilder.ToString();
        }

        public virtual string Update()
        {
            

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
                                         EntityMapping.TableName,
                                         string.Join(", ", EntityMapping.Properties.Where(x=>!x.IsIgnored && !x.IsPrimaryKey).Select(p => (!string.IsNullOrEmpty(p.ColumnName) ? $"[{EntityMapping.TableName}].[{p.ColumnName}]" : $"[{EntityMapping.TableName}].[{p.PropertyInfo.Name}]") + $" = @{p.PropertyInfo.Name}")),
                                         $"{EntityMapping.TableName}.{EntityMapping.PrimaryKey.ColumnName ?? EntityMapping.PrimaryKey.PropertyInfo.Name} = @{EntityMapping.PrimaryKey.PropertyInfo.Name}");

            return strBuilder.ToString();
        }

        public virtual string Select(Expression<Func<TEntity, bool>> predicate = null)
        {
            string ProjectionFunction(IPropertyMapping p) => !string.IsNullOrEmpty(p.ColumnName) ? $"{EntityMapping.TableName}.{p.ColumnName} AS {p.PropertyInfo.Name}" : $"{EntityMapping.TableName}.{p.PropertyInfo.Name}";

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("SELECT {0} FROM {1} ",
                                    string.Join(", ", EntityMapping.Properties.Select(ProjectionFunction)),
                                    EntityMapping.TableName);

            var sqlGenerator = new LambdaToSql<TEntity>(EntityMapping, new SqlCeBuilder() );

            if (predicate != null)
            {
                strBuilder.Append(sqlGenerator.GenerateWhere(predicate));
            }

            if (EntityMapping.IsDeletedProperty != null)
            {
                strBuilder.AppendFormat(predicate != null ? " AND {0}.{1} != {2}" : " WHERE {0}.{1} != {2}",
                    EntityMapping.TableName,
                    EntityMapping.IsDeletedProperty.PropertyInfo.Name,
                    0);
            }

            SelectParameters = sqlGenerator.SelectParameters;

            return strBuilder.ToString();
        }

        public virtual string Delete(bool forceDelete = false)
        {
            var strBuilder = new StringBuilder();

            if (forceDelete || EntityMapping.IsDeletedProperty == null)
            {
                strBuilder.AppendFormat("DELETE FROM {0} WHERE {1}",
                    EntityMapping.TableName,
                    string.Join(" AND ", $"{EntityMapping.TableName}.{EntityMapping.PrimaryKey.ColumnName ?? EntityMapping.PrimaryKey.PropertyInfo.Name} = @{EntityMapping.PrimaryKey.PropertyInfo.Name}"));
            }
            else
            {
                strBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2} ",
                    EntityMapping.TableName,
                 $"[{EntityMapping.TableName}].[{EntityMapping.IsDeletedProperty.ColumnName}] = 1",
                                 string.Join(" AND ", $"{EntityMapping.TableName}.{EntityMapping.PrimaryKey.ColumnName ?? EntityMapping.PrimaryKey.PropertyInfo.Name} = @{EntityMapping.PrimaryKey.PropertyInfo.Name}"));
            }

            return strBuilder.ToString();
        }

        #endregion
    }
}
