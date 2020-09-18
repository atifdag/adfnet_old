using ADF.Net.Data.BaseEntities;

namespace ADF.Net.Data.DataEntities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
