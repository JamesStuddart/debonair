using System;

namespace Debonair.Data.Orm
{
    public class KeyProperty : Attribute
    {
        public KeyProperty(bool identity)
        {
            Identity = identity;
        }
        public bool Identity { get; }
    }
    public class IsDeletedProperty : Attribute { }

    public class Ignore : Attribute { }

    public class Schema : Attribute
    {
        public string Value { get; }

        public Schema(string value)
        {
            Value = value;
        }
    }

    public class Table : Attribute
    {
        public string Value { get; }

        public Table(string value)
        {
            this.Value = value;
        }
    }

    public class Column : Attribute
    {
        public string Value { get;  }

        public Column(string value)
        {
            Value = value;
        }
    }

}
