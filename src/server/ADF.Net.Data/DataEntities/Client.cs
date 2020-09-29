using ADF.Net.Data.BaseEntities;

namespace ADF.Net.Data.DataEntities
{
    //Cari Hesap
    public class Client  : BaseEntity
    {
        
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Address { get; set; }

        public string Phone { get; set; }

        public string SpecialCode { get; set; }

        public string AuthorizedPerson { get; set; }

        public string RelatedPerson { get; set; }

        public string RelatedPersonPhone { get; set; }

        public virtual ClientType ClientType { get; set; }

        

    }
}
