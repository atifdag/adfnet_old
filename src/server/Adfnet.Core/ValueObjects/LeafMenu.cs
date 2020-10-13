using System;

namespace Adfnet.Core.ValueObjects
{
    public class LeafMenu
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public virtual ChildMenu Parent { get; set; }
    }
}
