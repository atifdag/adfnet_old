using System.Collections.Generic;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class ParameterGroup : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Parameter> Parameters { get; set; }
    }
}
