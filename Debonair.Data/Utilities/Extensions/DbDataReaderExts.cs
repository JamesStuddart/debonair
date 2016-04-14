using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Debonair.Data.Orm;

namespace Debonair.Utilities.Extensions
{
    public static class DbDataReaderExts
    {
        public static List<T> MapTo<T>(this IDataReader dr)
        {
            var list = new List<T>();
            while (dr.Read())
            {
                var obj = Activator.CreateInstance<T>();
                try
                {
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        var ignore = (Ignore)prop.GetCustomAttributes(typeof(Ignore), false).FirstOrDefault();
                        if (ignore != null) continue;

                        var attr = (Column)prop.GetCustomAttributes(typeof(Column), false).FirstOrDefault();

                        var colName = attr != null ? attr.Value : prop.Name;

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
