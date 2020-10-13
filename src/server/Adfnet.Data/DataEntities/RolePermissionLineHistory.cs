using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class RolePermissionLineHistory : LineBaseHistoryEntity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
