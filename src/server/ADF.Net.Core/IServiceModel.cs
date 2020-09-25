using System;

namespace ADF.Net.Core
{
    public interface IServiceModel
    {
        Guid Id { get; set; }
        int DisplayOrder { get; set; }
        bool IsApproved { get; set; }
        int Version { get; set; }
        DateTime CreationTime { get; set; }
        DateTime LastModificationTime { get; set; }
    }
}
