using System;
using Adfnet.Core;

namespace Adfnet.Data.DataEntities
{

    public class SessionHistory : IEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Oturum bitiriþ türü
        /// </summary>
        public string LogoutType { get; set; }

        /// <summary>
        /// Oluþturulma zamaný
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Son deðiþiklik zamaný
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// Oluþturan kullanýcý
        /// </summary>
        public virtual User Creator { get; set; }

    }
}
