using System;

namespace ADF.Net.Core.Exceptions
{
    public abstract class BaseApplicationException : ApplicationException
    {
        protected BaseApplicationException(string message) : base(message)
        {

        }
    }
}
