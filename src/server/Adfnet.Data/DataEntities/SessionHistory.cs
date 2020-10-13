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
        /// Oturum bitiri� t�r�
        /// </summary>
        public string LogoutType { get; set; }

        /// <summary>
        /// Olu�turulma zaman�
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Son de�i�iklik zaman�
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// Olu�turan kullan�c�
        /// </summary>
        public virtual User Creator { get; set; }

    }
}
