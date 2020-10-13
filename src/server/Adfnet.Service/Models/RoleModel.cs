using System;
using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class RoleModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdCodeName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdCodeName LastModifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public List<IdCodeNameSelected> Permissions { get; set; }

    }
}
