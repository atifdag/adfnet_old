using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı CategoryHistory tablosu konfigürasyonu
    /// </summary>
    internal class CategoryHistoryConfiguration : IEntityTypeConfiguration<CategoryHistory>
    {
        public void Configure(EntityTypeBuilder<CategoryHistory> builder)
        {
            // Tablo adı
            builder.ToTable("CategoryHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();

        }
    }
}
