using System;
using Adfnet.Core.ValueObjects;

namespace Adfnet.Service.Models
{
    public class UpdateInformationModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreationTime { get; set; }
        public IdName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdName LastModifier { get; set; }
        public IdCodeName Language { get; set; }
    }
}
