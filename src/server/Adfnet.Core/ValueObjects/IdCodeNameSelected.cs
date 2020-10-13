using System;

namespace Adfnet.Core.ValueObjects
{
    public struct IdCodeNameSelected
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }

        public IdCodeNameSelected(Guid id, string code, string name, bool selected)
        {
            Id = id;
            Code = code;
            Name = name;
            Selected = selected;
        }
    }
}
