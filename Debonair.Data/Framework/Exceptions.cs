using System;

namespace Debonair.Framework
{

    public class UnableToUpdateEntityException : Exception
    {
    }

    public class DependancyNotFoundException : Exception
    {
        public DependancyNotFoundException(string message) : base(message) { }
    }
    public class MappingNotFoundException : Exception
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
