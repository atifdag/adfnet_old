using System;
using Adfnet.Core;

namespace Adfnet.Data.BaseEntities
{
    /// <inheritdoc />
    /// <summary>
    /// Değişiklik geçmişi bilgisine sahip çoktan çoka ilişkli veri tabanı nesneleri için temel nesne
    /// </summary>
    public class LanguageLineBaseHistoryEntity : BaseHistoryEntity
    {
        /// <summary>
        /// Kopyası tutulan referans satırın dil Id'si
        /// </summary>
        public Guid LanguageId { get; set; }

    }

}
