﻿using System;
using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class PermissionModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdName LastModifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public List<IdCodeNameSelected> Menus { get; set; }


    }
}
