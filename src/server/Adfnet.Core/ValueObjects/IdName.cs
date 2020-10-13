using System;

namespace Adfnet.Core.ValueObjects
{
    public struct IdName
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IdName(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
