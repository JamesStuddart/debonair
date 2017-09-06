using System;

namespace Debonair.Tests.MockObjects
{
    public class TestObject
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public TestStatus Status { get; set; }
    }

    public class DeleteableTestObject 
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public TestStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }

    public enum TestStatus
    {
        Empty = 0,
        Good = 100,
        Dead = 200
    }

}
