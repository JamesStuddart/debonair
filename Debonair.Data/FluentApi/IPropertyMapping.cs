using System.Reflection;

namespace Debonair.FluentApi
{
    public interface IPropertyMapping
    {
        PropertyInfo PropertyInfo { get; set; }

        bool IsPrimaryKey { get; }
        bool IsDeletedProperty { get; }
        bool IsIgnored { get; }
        string ColumnName { get;  }


        #region set methods

        void SetPrimaryKey();
        void SetIsDeletedProperty();
        void Ignore();
        void SetColumnName(string columnName);

        #endregion set methods
    }
}