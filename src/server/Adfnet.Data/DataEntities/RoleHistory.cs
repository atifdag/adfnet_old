using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class RoleHistory : BaseHistoryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }

    }
}