using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class Product : BaseEntity
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public virtual Category Category { get; set; }

    }
}
