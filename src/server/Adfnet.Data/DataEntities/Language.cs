using System;
using System.Collections.Generic;
using Adfnet.Core;

namespace Adfnet.Data.DataEntities
{
    public class Language : IEntity
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime LastModificationTime { get; set; }
        public Guid LastModifierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLines { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
