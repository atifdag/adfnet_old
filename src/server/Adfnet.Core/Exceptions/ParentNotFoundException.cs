using Adfnet.Core.Globalization;

namespace Adfnet.Core.Exceptions
{

    public class ParentNotFoundException : BaseApplicationException
    {
        public ParentNotFoundException() : base(Messages.DangerParentNotFound)
        {
        }

     
    }
}