using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using Debonair.Framework;

namespace Debonair.FluentApi
{
    public static class EntityMappingEngine
    {
        private static bool isInitialized = false;
        private static List<IEntityMapping> entityMappings;

        public static void Initialize(List<IEntityMapping> mappings = null)
        {
            try
            {
                entityMappings = mappings ?? LoadEntityMappings();
                isInitialized = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<IEntityMapping> LoadEntityMappings()
        {
            //go get all the mappings
            return new List<IEntityMapping>();
        }

        public static IEntityMapping<TEntity> GetMappingForEntity<TEntity>() where TEntity : class
        {
            CheckInitialization();

            var mapping = (IEntityMapping<TEntity>) entityMappings.FirstOrDefault(x => x.GetType() == typeof(IEntityMapping<TEntity>));

            if (mapping == null)
            {
                mapping = new EntityMapping<TEntity>();

                mapping.SetTableName(typeof(TEntity).Name);
                mapping.SetSchemaName("dbo");
            }

            return mapping;
        }


        private static void CheckInitialization()
        {
            if (isInitialized == false)
                Initialize();
            //throw new InitializationException("Debonair Entity Mapping Engine NOT Initialized");
        }
    }
}
