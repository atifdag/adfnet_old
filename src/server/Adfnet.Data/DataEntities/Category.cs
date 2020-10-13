using System.Collections.Generic;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class Category : LanguageBaseEntity
    {
        public string Code { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLines { get; set; }
    }
}
