using System;
using System.Linq;
using System.Reflection;
using Debonair.Data.Orm;

namespace Debonair.Entities
{
    public class DebonairStandard
    {
        [Ignore]
        public int? PrimaryKey
        {
            get
            {
                var entityType = typeof (DebonairStandard);
                var props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType).ToList();
                return
                    (int?)
                        props.SingleOrDefault(p => p.GetCustomAttributes<KeyProperty>().Any(k => k.Identity))?
                            .GetValue(this);
            }
            set
            {
                var entityType = typeof (DebonairStandard);
                var props = entityType.GetProperties().Where(p => p.PropertyType.IsValueType).ToList();
                props.SingleOrDefault(p => p.GetCustomAttributes<KeyProperty>().Any(k => k.Identity))?
                    .SetValue(this, value);
            }
        }
    }

    public class DebonairDeleteable : DebonairStandard
        {
            [IsDeletedProperty]
            public bool IsDeleted { get; set; }
        }


    }
