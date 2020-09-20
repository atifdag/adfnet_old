using ADF.Net.Core.Globalization;

namespace ADF.Net.Core.Exceptions
{

    public class NotFoundException : BaseApplicationException
    {
        public NotFoundException() : base(Messages.DangerRecordNotFound)
        {
        }

     
    }
}