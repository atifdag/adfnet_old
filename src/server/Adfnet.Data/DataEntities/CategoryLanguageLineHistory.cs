using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    public class CategoryLanguageLineHistory : LanguageLineBaseHistoryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}
