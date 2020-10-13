using System;
using Adfnet.Data.BaseEntities;

namespace Adfnet.Data.DataEntities
{
    /// <inheritdoc />
    /// <summary>
    /// Menu sınıfı için tarihçe
    /// </summary>
    public class MenuHistory : BaseHistoryEntity
    {

        /// <summary>
        /// Menunun kodu
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Menunun görünen adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Menunun adresi
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Menunun ikon bilgisi
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Menunun açıklaması
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Menunun üst menu id bilgisi
        /// </summary>
        public Guid ParentMenuId { get; set; }


    }
}
