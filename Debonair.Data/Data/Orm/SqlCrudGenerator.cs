using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Debonair.Data.Orm.QueryBuilder;
using Debonair.Entities;

namespace Debonair.Data.Orm
{
    public class SqlCrudGenerator<TEntity> : ICrudGenerator<TEntity> where TEntity : DebonairStandard, new()
    {
        public Dictionary<string, object> SelectParameters { get; set; }

        public bool IsPrimaryKey => IdentityProperty != null;

        public string TableName { get; private set; }

        public string Scheme { get; private set; }

        public PropertyMetadata IdentityProperty { get; private set; }

        public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

        public IEnumerable<PropertyMetadata> BaseProperties { get; private set; }

        public PropertyMetadata IsDeletedProperty { get; private set; }

        public bool IsSoftDeletable { get; private set; }

        public SqlCrudGenerator()
        {
            GetEntityMetadata();
        }

        private void GetEntityMetadata()
        {
            var entityType = typeof(TEntity);

            var tableAttribute = entityType.GetCustomAttribute<Table>();
            var schemeAttribute = entityType.GetCustomAttribute<Schema>();

            TableName = tableAttribute != null ? tableAttribute.Value : entityType.Name;

            Scheme = schemeAttribute != null ? schemeAttribute.Value : "dbo";

            var props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType ||
                                                              p.PropertyType.Name.Equals("String", StringComparison.InvariantCultureIgnoreCase) ||
                                                              p.PropertyType.Name.Equals("Byte[]", StringComparison.InvariantCultureIgnoreCase)).ToList();

            BaseProperties = props.Where(p => !p.GetCustomAttributes<Ignore>().Any()).Select(p => new PropertyMetadata(p));

            KeyProperties = props.Where(p => p.GetCustomAttributes<KeyProperty>().Any()).Select(p => new PropertyMetadata(p));

            var identityProperty = props.SingleOrDefault(p => p.GetCustomAttributes<KeyProperty>().Any(k => k.Identity));
            IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null;

            var deleteProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IsDeletedProperty>().Any());
            IsSoftDeletable = deleteProperty != null;

            if (IsSoftDeletable)
                IsDeletedProperty = new PropertyMetadata(deleteProperty);
        }

   

        #region Query generators

        public virtual string Insert()
        {

            if (typeof(IEnumerable).IsAssignableFrom(typeof(TEntity)) && !typeof(string).IsAssignableFrom(typeof(TEntity)))
            {
                throw new NotSupportedException("To insert multiple lines of data it is faster and more effcient to use SQLBulkCopy ");
            }

            IEnumerable<PropertyMetadata> properties = (IsPrimaryKey ?
                                                        BaseProperties.Where(p => !p.Name.Equals(IdentityProperty.Name, StringComparison.InvariantCultureIgnoreCase)) :
                                                        BaseProperties).ToList();

            var columNames = string.Join(", ", properties.Select(p => $"[{TableName}].[{p.ColumnName}]"));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("INSERT INTO [{0}].[{1}] {2} {3} ",
                                    Scheme,
                                    TableName,
                                    string.IsNullOrEmpty(columNames) ? string.Empty : $"({columNames})",
                                    string.IsNullOrEmpty(values) ? string.Empty : $" VALUES ({values})");

            if (IsPrimaryKey)
            {
                strBuilder.AppendLine("DECLARE @NEWID NUMERIC(38, 0)");
                strBuilder.AppendLine("SET @NEWID = SCOPE_IDENTITY()");
                strBuilder.AppendLine("SELECT @NEWID");
            }

            return strBuilder.ToString();
        }

        public virtual string Update()
        {
            var properties = BaseProperties.Where(p => !KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)));

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("UPDATE [{0}].[{1}] SET {2} WHERE {3}",
                                    Scheme,
                                    TableName,
                                    string.Join(", ", properties.Select(p =>
                                        $"[{TableName}].[{p.ColumnName}] = @{p.Name}")),
                                    string.Join(" AND ", KeyProperties.Select(p =>
                                        $"[{TableName}].[{p.ColumnName}] = @{p.Name}")));

            return strBuilder.ToString();
        }

        public virtual string Select(Expression<Func<TEntity, bool>> predicate = null, bool dirtyRead = true)
        {
            Func<PropertyMetadata, string> projectionFunction = p => !string.IsNullOrEmpty(p.TableName) ? $"[{TableName}].[{p.ColumnName}] AS [{p.Name}]" : $"[{TableName}].[{p.ColumnName}]";

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("SELECT {0} FROM [{1}].[{2}] " + (dirtyRead ? "WITH (NOLOCK)" : string.Empty),
                                    string.Join(", ", BaseProperties.Select(projectionFunction)),
                                    Scheme,
                                    TableName);

            var sqlGenerator = new LambdaToSql<TEntity>();

            if (predicate != null)
            {
                strBuilder.Append(sqlGenerator.GenerateWhere(predicate));
                SelectParameters = sqlGenerator.SelectParameters;
            }

            if (IsSoftDeletable && !sqlGenerator.ContainsIsDeleteable)
            {
                strBuilder.AppendFormat(predicate != null ? " AND [{0}].[{1}] != {2}" : " WHERE [{0}].[{1}] != {2}",
                    TableName,
                    IsDeletedProperty.Name,
                    0);
            }


            return strBuilder.ToString();
        }



        public virtual string Delete(bool forceDelete = false)
        {
            var strBuilder = new StringBuilder();

            if (forceDelete || !IsSoftDeletable)
            {
                strBuilder.AppendFormat("DELETE FROM [{0}].[{1}] WHERE {2}",
                    Scheme,
                    TableName,
                    string.Join(" AND ", KeyProperties.Select(p =>
                        $"[{TableName}].[{p.ColumnName}] = @{p.Name}")));

            }
            else
            {
                strBuilder.AppendFormat("UPDATE [{0}].[{1}] SET {2} WHERE {3}",
                                 Scheme,
                                 TableName,
                 $"[{TableName}].[{IsDeletedProperty.ColumnName}] = 1",
                                 string.Join(" AND ", KeyProperties.Select(p =>
                                     $"[{TableName}].[{p.ColumnName}] = @{p.Name}")));
            }

            return strBuilder.ToString();
        }

        #endregion
    }
}
