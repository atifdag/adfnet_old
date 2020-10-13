using System;
using Adfnet.Core;
using Adfnet.Data.DataEntities;

namespace Adfnet.Data.BaseEntities
{
    /// <inheritdoc />
    /// <summary>
    /// Çoktan çoka ilişkli veri tabanı nesneleri için temel nesne
    /// </summary>
    public class LanguageLineBaseEntity : BaseEntity
    {

        public virtual Language Language { get; set; }

    }

}
