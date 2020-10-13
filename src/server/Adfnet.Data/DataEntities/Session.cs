using System;
using Adfnet.Core;

namespace Adfnet.Data.DataEntities
{

    public class Session : IEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Olu�turulma zaman�
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Olu�turan kullan�c�
        /// </summary>
        public virtual User Creator { get; set; }

    }
}
