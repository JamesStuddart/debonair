using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Debonair.Utilities.Extensions
{
    public static class New<TEntity> where TEntity : class, new()
    {
        public static readonly Func<TEntity> Instance = Expression.Lambda<Func<TEntity>>(Expression.New(typeof(TEntity))).Compile();
    }

    public static class DbDataReaderExts
    {
        private static HashSet<int> _colHashSet = new HashSet<int>();
        private static string _tableName;

        public static List<TEntity> MapTo<TEntity>(this IDataReader dr) where TEntity : class, new()
        {
            var list = new List<TEntity>();

            while (dr.Read())
            {
                //var c = typeof(TEntity)
                //    .GetTypeInfo()
                //    .DeclaredConstructors
                //    .Single(ci => ci.GetParameters().Length == 0);
                //var obj = (TEntity) c.Invoke(Type.EmptyTypes);

                // var obj = Activator.CreateInstance<TEntity>();

                var obj = New<TEntity>.Instance();

                foreach (var prop in EntityCache.GetPropertyInfo(obj))
                {
                    var colName = prop.Name;

                    if (!HasColumn(dr, colName) || MappingCache.GetPropertyMapping<TEntity>(prop).IsIgnored) continue;

                    if (!Equals(dr[colName], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[colName]);
                    }
                }

                list.Add(obj);
            }
            return list;
        }


        private static bool HasColumn(IDataRecord dr, string colName)
        {
            try
            {
                return dr.GetOrdinal(colName) >= 0;
            }
            catch
            {
                return false;
            }
        }
        
    }


}
