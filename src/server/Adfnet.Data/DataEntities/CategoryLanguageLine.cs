using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class CategoryLanguageLine : LanguageLineBaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Category Category { get; set; }

    }
}
