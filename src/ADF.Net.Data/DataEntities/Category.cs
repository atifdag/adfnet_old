using System.Collections.Generic;
using ADF.Net.Data.BaseEntities;

namespace ADF.Net.Data.DataEntities
{
    public class Category : BaseEntity
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        
    }
}
