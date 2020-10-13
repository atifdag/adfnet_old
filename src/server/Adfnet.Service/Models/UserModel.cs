using System;
using System.Collections.Generic;
using Adfnet.Core;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class UserModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdName LastModifier { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<IdCodeNameSelected> Roles { get; set; }
        public string IdentityCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName => FirstName + " " + LastName;
        public IdCodeName Language { get; set; }

    }
}
