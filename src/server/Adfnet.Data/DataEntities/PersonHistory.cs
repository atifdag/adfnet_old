using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class PersonHistory : BaseHistoryEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityCode { get; set; }
    }
}
