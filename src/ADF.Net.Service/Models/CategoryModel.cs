﻿using System;
using ADF.Net.Core;

namespace ADF.Net.Service.Models
{
    public class CategoryModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
