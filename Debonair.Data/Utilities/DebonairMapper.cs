using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Debonair.Utilities
{
    public static class DebonairMapper
    {
        /// <typeparam name="TEntity">Objet you want to map TO</typeparam>
        /// <param name="source">Objet you want to map FROM</param>
        /// <param name="strict">Ensure property types match as well as names</param>
        /// <returns></returns>
        public static TEntity MapTo<TEntity>(this object source, bool strict = true) where TEntity : class, new()
        {
            var destination = new TEntity();
            var sourceProperties = source.GetType().GetProperties();
            var destionationProperties = destination.GetType().GetProperties();

            var commonproperties = strict ? (from x in sourceProperties
                                             join y in destionationProperties on new { x.Name, x.PropertyType } equals
                                                 new { y.Name, y.PropertyType }
                                             select new { x, y }) : (from x in sourceProperties
                                                                     join y in destionationProperties on new { Name = x.Name.ToLower() } equals
                                                                         new { Name = y.Name.ToLower() }
                                                                     select new { x, y });

            foreach (var match in commonproperties)
            {
                try
                {
                    match.y.SetValue(destination, match.x.GetValue(source, null), null);
                }
                catch
                {
                    //match didn't work, we'll just ignore them for now
                    //TODO: this needs fixing
                }
            }

            return destination;
        }

        public static List<IDbDataParameter> ToDbDataParameters<TEntity>(this object source, IDbConnection connection) where TEntity : class, new()
        {
            var list = new List<IDbDataParameter>();

            foreach (var prop in source.GetType().GetProperties())
            {
                if (MappingCache.GetPropertyMapping<TEntity>(prop).IsIgnored) continue;

                list.Add(CreateParameter(connection, prop.Name, prop.GetValue(source, null)));
            }

            return list;
        }

        public static List<IDbDataParameter> ToDbDataParameters(this Dictionary<string, object> source, IDbConnection connection)
        {
            return source?.Select(item => CreateParameter(connection, item.Key, item.Value)).ToList() ?? new List<IDbDataParameter>();
        }

        private static IDbDataParameter CreateParameter(IDbConnection connection, string name, object value)
        {
            var paramater = connection.CreateCommand().CreateParameter();

            paramater.ParameterName = name;
            paramater.Value = value;
            paramater.DbType = GetDbType(value.GetType());
            return paramater;
        }

        private static DbType GetDbType(Type type)
        {
            var strTypeName = type.Name;
            var dbType = DbType.String;

            if (ReferenceEquals(type, typeof(DBNull)))
            {
                return dbType;
            }

            if (ReferenceEquals(type, typeof(byte[])))
            {
                return DbType.Binary;
            }

            dbType = (DbType)Enum.Parse(typeof(DbType), strTypeName, true);

            switch (dbType)
            {
                case DbType.UInt64:
                    dbType = DbType.Int64;
                    break;
                case DbType.UInt32:
                    dbType = DbType.Int32;
                    break;
            }

            return dbType;
        }
    }
}
