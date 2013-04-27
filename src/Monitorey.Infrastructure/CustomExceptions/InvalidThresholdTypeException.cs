using System;
using System.Runtime.Serialization;

namespace monitory.Infrastructure.CustomExceptions
{
    public class InvalidThresholdTypeException : Exception
    {
        public InvalidThresholdTypeException()
        {
        }

        public InvalidThresholdTypeException(string message) : base(message)
        {
        }

        public InvalidThresholdTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidThresholdTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}