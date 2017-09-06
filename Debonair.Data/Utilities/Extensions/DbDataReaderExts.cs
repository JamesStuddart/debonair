using System;
using System.Collections.Generic;
using System.Data;

namespace Debonair.Utilities.Extensions
{
    public static class DbDataReaderExts
    {
        private static HashSet<int> ColHashSet = new HashSet<int>();
        private static string TableName;

        public static List<TEntity> MapTo<TEntity>(this IDataReader dr) where TEntity : class, new()
        {
            var list = new List<TEntity>();

            GetColumnNames(dr, typeof(TEntity).FullName);

            while (dr.Read())
            {
                var obj = Activator.CreateInstance<TEntity>();

                foreach (var prop in EntityCache.GetPropertyInfo(obj))
                {
                    var colName = prop.Name;

                    if (!HasColumn(colName) || MappingCache.GetPropertyMapping<TEntity>(prop).IsIgnored) continue;

                    if (!Equals(dr[colName], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[colName]);
                    }
                }

                list.Add(obj);
            }
            return list;
        }


        private static bool HasColumn(string colName)
        {
            return ColHashSet.Contains(colName.ToLower().GetHashCode());
        }

        private static void GetColumnNames(IDataReader dr, string tableName)
        {
            if (string.IsNullOrEmpty(TableName) || !TableName.Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
            {
                ColHashSet = new HashSet<int>();
                TableName = tableName;

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    ColHashSet.Add(dr.GetName(i).ToLower().GetHashCode());
                }
            }
        }
    }


}
