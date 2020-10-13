using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{

    public class LanguageHistory : BaseHistoryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
