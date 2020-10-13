using System;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class PersonModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdCodeName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdCodeName LastModifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName => FirstName + " " + LastName;
        public string IdentityCode { get; set; }
        public string Biography { get; set; }

    }
}
