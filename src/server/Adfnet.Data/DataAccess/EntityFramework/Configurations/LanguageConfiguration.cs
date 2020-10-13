using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı Language tablosu konfigürasyonu
    /// </summary>
    internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            // Tablo adı
            builder.ToTable("Languages");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_LanguageDisplayOrder");
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.LastModifierId).IsRequired();
            builder.Property(x => x.LastModificationTime).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Code).IsUnique().HasName("UK_LanguageCode");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");


        }
    }
}
