using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný PersonHistory tablosu konfigürasyonu
    /// </summary>
    internal class CategoryLanguageLineHistoryConfiguration : IEntityTypeConfiguration<CategoryLanguageLineHistory>
    {
        public void Configure(EntityTypeBuilder<CategoryLanguageLineHistory> builder)
        {
            builder.ToTable("CategoryLanguageLineHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");
            builder.Property(x => x.CategoryId).IsRequired();

            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.LanguageId).IsRequired();

        }
    }
}
