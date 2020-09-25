using System.Collections.Generic;
using ADF.Net.Data.BaseEntities;

namespace ADF.Net.Data.DataEntities
{
    //Cari Hesap Türü
    public class ClientType : BaseEntity
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
        
    }
}


