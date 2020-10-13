using Adfnet.Core.Globalization;

namespace Adfnet.Core.Exceptions
{

    public class NotFoundException : BaseApplicationException
    {
        public NotFoundException() : base(Messages.DangerRecordNotFound)
        {
        }

        public NotFoundException(string message) : base(message)
        {

        }

    }
}