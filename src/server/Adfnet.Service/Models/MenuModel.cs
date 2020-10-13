using System;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class MenuModel : IServiceModel
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
        public string Address { get; set; }
        public string Icon { get; set; }
        public IdCodeName ParentMenu { get; set; }


    }
}
