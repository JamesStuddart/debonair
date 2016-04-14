using System;
using Debonair.Data.Orm;
using Debonair.Entities;

namespace Debonair.Tests.MockObjects
{
    public class TestObject : DebonairStandard
    {
        [KeyProperty(true)]
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public TestStatus Status { get; set; }
    }

    [Table("TestTable")]
    public class DeleteableTestObject : DebonairDeleteable
    {
        [KeyProperty(true)]
        public int Id { get; set; }
        [Column("Name")]
        public string CustomerName { get; set; }
        public TestStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public enum TestStatus
    {
        empty = 0,
        good = 100,
        dead = 200
    }

}
