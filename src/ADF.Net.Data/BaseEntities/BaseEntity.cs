using System;
using ADF.Net.Core;

namespace ADF.Net.Data.BaseEntities
{
    public class BaseEntity : IEntity
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsApproved { get; set; }

        public int Version { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

    }

}
