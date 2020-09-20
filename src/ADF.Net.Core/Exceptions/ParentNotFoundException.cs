using ADF.Net.Core.Globalization;

namespace ADF.Net.Core.Exceptions
{

    public class ParentNotFoundException : BaseApplicationException
    {
        public ParentNotFoundException() : base(Messages.DangerParentNotFound)
        {
        }

     
    }
}