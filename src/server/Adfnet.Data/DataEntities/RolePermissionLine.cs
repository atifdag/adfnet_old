using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class RolePermissionLine : LineBaseEntity
    {
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
