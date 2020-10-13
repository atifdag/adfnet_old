using System;
using System.Collections.Generic;

namespace Adfnet.Core.ValueObjects
{
    public class ChildMenu
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public virtual ICollection<LeafMenu> LeafMenus { get; set; }
        public virtual RootMenu Parent { get; set; }
    }
}
