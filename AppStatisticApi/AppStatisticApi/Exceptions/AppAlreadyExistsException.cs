using System;
using System.Reflection;

namespace AppStatisticApi.Exceptions
{
    public class AppAlreadyExistsException: Exception
    {
        public AppAlreadyExistsException()
        { }

        public AppAlreadyExistsException(string message)
            : base(message)
        { }

        public AppAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        { }

        public static bool checkUniqConstrainError(Exception e)
        {
            Type innerExceptionType = e.InnerException.GetType();
            PropertyInfo codeProperty = innerExceptionType.GetProperty("Code");
            string codeValue = (string)codeProperty.GetValue(e.InnerException);

            return codeValue == "23505";
        }
    }
}
