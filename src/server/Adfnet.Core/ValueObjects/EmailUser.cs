using System;

namespace Adfnet.Core.ValueObjects
{
    public struct EmailUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
