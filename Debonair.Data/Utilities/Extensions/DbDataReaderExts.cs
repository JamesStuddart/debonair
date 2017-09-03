using System;
using System.Collections.Generic;
using System.Data;

namespace Debonair.Utilities.Extensions
{
    public static class DbDataReaderExts
    {
        private static readonly EntityCache EntityCache = new EntityCache();
        private static readonly MappingCache MappingCache = new MappingCache();

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
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }


}
