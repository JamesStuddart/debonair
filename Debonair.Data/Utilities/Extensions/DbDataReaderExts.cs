using System;
using System.Collections.Generic;
using System.Data;
using Debonair.FluentApi;

namespace Debonair.Utilities.Extensions
{
    public static class DbDataReaderExts
    {
        public static List<TEntity> MapTo<TEntity>(this IDataReader dr) where TEntity : class, new()
        {
            var list = new List<TEntity>();
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<TEntity>();
                try
                {
                    var mapping = EntityMappingEngine.GetMappingForEntity<TEntity>();

                        foreach (var prop in obj.GetType().GetProperties())
                    {
                        var propMapping = mapping.GetMappingForType(prop);

                        if (propMapping.IsIgnored) continue;

                        var colName = prop.Name;
                        
                        if (!HasColumn(dr, colName)) continue;

                        if (!Equals(dr[colName], DBNull.Value))
                        {
                            prop.SetValue(obj, dr[colName]);
                        }
                    }
                    list.Add(obj);

                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            return list;
        }

        private static bool HasColumn(this IDataRecord dr, string colName)
        {
            for (var i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(colName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }


}
