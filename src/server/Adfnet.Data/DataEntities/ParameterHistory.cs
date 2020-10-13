using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{

    public class ParameterHistory : BaseHistoryEntity
    {
        public Guid ParameterGroupId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Erasable { get; set; }
        public string Description { get; set; }

    }
}
