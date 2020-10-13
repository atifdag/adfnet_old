using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class PermissionMenuLine : LineBaseEntity
    {
        public virtual Permission Permission { get; set; }
        public virtual Menu Menu { get; set; }
    }
}
