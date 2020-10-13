using System;
using System.Collections.Generic;

namespace Adfnet.Core.ValueObjects
{
    public class RootMenu
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ChildMenu> ChildMenus { get; set; }
    }
}
