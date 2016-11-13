using System;
using System.Management.Instrumentation;

namespace Debonair.Framework
{

    public class UnableToUpdateEntityException : Exception
    {
    }

    public class DependancyNotFoundException : InstanceNotFoundException
    {
        public DependancyNotFoundException(string message) : base(message) { }
    }
    public class MappingNotFoundException : InstanceNotFoundException
    {
        public MappingNotFoundException(string message) : base(message) { }
    }

    public class InsufficientInformationException : Exception
    {
        public InsufficientInformationException(string message) : base(message) { }
    }

    public class InitializationException : Exception
    {
        public InitializationException(string message, Exception innerException = null) : base(message, innerException) { }
    }
}
