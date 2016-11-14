using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Debonair.FluentApi
{
    public static class EntityMappingEngine
    {
        private static bool isInitialized = false;
        private static IList<IEntityMapping> entityMappings;

        public static void Initialize(IList<IEntityMapping> mappings = null)
        {
            ForceLoadAssemblies();

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

        public static IEntityMapping<TEntity> GetMappingForEntity<TEntity>() where TEntity : class
        {
            CheckInitialization();

            var mapping = (IEntityMapping<TEntity>)entityMappings.Where(x => x.GetType().BaseType != null).FirstOrDefault(x => x.GetType().BaseType.GenericTypeArguments[0] == typeof(TEntity));

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


        private static IList<IEntityMapping> LoadEntityMappings()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(IEntityMapping).IsAssignableFrom(x) && !x.IsGenericType && !x.IsGenericTypeDefinition && !x.IsInterface).Select(x => (IEntityMapping)Activator.CreateInstance(x)).ToList();
        }

        private static void ForceLoadAssemblies()
        {
            var stackFrames = new StackTrace().GetFrames();
            if (stackFrames != null)
                foreach (var frame in stackFrames)
                {
                    var type = frame.GetMethod().DeclaringType;
                    if (type != null)
                    {
                        foreach (var refedAssembly in type.Assembly.GetReferencedAssemblies())
                        {
                            Assembly.Load(refedAssembly);
                        }
                    }
                }
        }
    }
}
