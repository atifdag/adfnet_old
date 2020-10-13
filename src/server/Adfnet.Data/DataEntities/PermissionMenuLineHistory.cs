using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class PermissionMenuLineHistory : LineBaseHistoryEntity
    {
        public Guid PermissionId { get; set; }
        public Guid MenuId { get; set; }
    }
}
