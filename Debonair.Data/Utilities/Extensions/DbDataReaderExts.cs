using System;
using System.Collections.Generic;
using System.Data;

namespace Debonair.Utilities.Extensions
{
    public static class DbDataReaderExts
    {
        private static readonly EntityCache EntityCache = new EntityCache();
        private static readonly MappingCache MappingCache = new MappingCache();
        private static readonly HashSet<int> ColHashSet = new HashSet<int>();
        private static string _tableName;

        public static List<TEntity> MapTo<TEntity>(this IDataReader dr) where TEntity : class, new()
        {
            var list = new List<TEntity>();
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<TEntity>();

                    foreach (var prop in EntityCache.GetPropertyInfo(obj))
                    {
                        var colName = prop.Name;

                        if (!HasColumn(dr, colName) || MappingCache.GetMapping<TEntity>(prop).IsIgnored) continue;

                        if (!Equals(dr[colName], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[colName]);
                        }
                    }

                    list.Add(obj);
            }
            return list;
        }


        public static bool HasColumn(IDataReader reader, string columnName)
        {
            var tblName = reader.GetSchemaTable().TableName;

            if (ColHashSet.Count > 0 && _tableName != tblName)
            {
                _tableName = tblName;

                foreach (DataColumn column in reader.GetSchemaTable().Columns)
                {
                    ColHashSet.Add(column.ColumnName.ToLower().GetHashCode());
                }
            }

            return ColHashSet.Contains(columnName.ToLower().GetHashCode());
        }
    }


}
