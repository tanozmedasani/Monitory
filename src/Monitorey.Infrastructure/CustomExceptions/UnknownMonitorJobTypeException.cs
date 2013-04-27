using System;
using System.Runtime.Serialization;

namespace monitory.Infrastructure.CustomExceptions
{
    public class UnknownMonitorJobTypeException : Exception
    {
        public UnknownMonitorJobTypeException()
        {
        }

        public UnknownMonitorJobTypeException(string message) : base(message)
        {
        }

        public UnknownMonitorJobTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownMonitorJobTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}