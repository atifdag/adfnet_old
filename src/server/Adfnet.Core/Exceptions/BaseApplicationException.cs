using System;

namespace Adfnet.Core.Exceptions
{
    public abstract class BaseApplicationException : ApplicationException
    {
        protected BaseApplicationException(string message) : base(message)
        {

        }
    }
}
