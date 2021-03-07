using System;

namespace AppStatisticGrpc.Exceptions
{
    public class InvalidAppUrlException: Exception
    {
        public InvalidAppUrlException()
        {
        }

        public InvalidAppUrlException(string message)
            : base(message)
        {
        }

        public InvalidAppUrlException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
