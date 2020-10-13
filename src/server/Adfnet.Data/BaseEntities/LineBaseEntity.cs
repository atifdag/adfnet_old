using System;
using Adfnet.Core;
using Adfnet.Data.DataEntities;

namespace Adfnet.Data.BaseEntities
{
    /// <inheritdoc />
    /// <summary>
    /// Çoktan çoka ilişkli veri tabanı nesneleri için temel nesne
    /// </summary>
    public class LineBaseEntity : IEntity
    {
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Sıra No
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Sürüm No
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Oluşturulma zamanı
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Oluşturan kullanıcı
        /// </summary>
        public virtual User Creator { get; set; }

        /// <summary>
        /// Son değişiklik zamanı
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// Son değiştiren kullanıcı
        /// </summary>
        public virtual User LastModifier { get; set; }

    }

}
