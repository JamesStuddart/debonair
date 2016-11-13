using System.Reflection;

namespace Debonair.FluentApi
{
    public class PropertyMapping : IPropertyMapping
    {
        public PropertyInfo PropertyInfo { get; set; }

        public PropertyMapping(PropertyInfo info)
        {
            PropertyInfo = info;
        }

        public bool IsPrimaryKey { get; private set; }
        public bool IsDeletedProperty { get; private set; }
        public bool IsIgnored { get; private set; }
        public string ColumnName { get; private set; }


        #region set methods
        public void SetPrimaryKey()
        {
            IsPrimaryKey = true;
        }

        public void SetIsDeletedProperty()
        {
            IsDeletedProperty = true;
        }

        public void Ignore()
        {
            IsIgnored = true;
        }

        public void SetColumnName(string columnName)
        {
            ColumnName = columnName;
        }
        #endregion set methods


        #region Validation properties

        public bool IsCaseSensitive { get; private set; }
        public bool IsRequired { get; private set; }

        #endregion Validation properties
    }
}
