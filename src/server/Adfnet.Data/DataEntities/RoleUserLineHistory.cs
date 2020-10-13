using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class RoleUserLineHistory : LineBaseHistoryEntity
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
