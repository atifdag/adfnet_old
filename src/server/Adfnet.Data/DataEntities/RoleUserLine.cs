using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class RoleUserLine : LineBaseEntity
    {
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
