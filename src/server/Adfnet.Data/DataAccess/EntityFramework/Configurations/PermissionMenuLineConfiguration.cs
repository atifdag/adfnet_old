using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný PermissionMenuLine tablosu konfigürasyonu
    /// </summary>
    internal class PermissionMenuLineConfiguration : IEntityTypeConfiguration<PermissionMenuLine>
    {
        public void Configure(EntityTypeBuilder<PermissionMenuLine> builder)
        {
            builder.ToTable("PermissionMenuLines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_PermissionMenuLineDisplayOrder");
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.PermissionMenuLinesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.PermissionMenuLinesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Permission).WithMany(y => y.PermissionMenuLines).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Menu).WithMany(y => y.PermissionMenuLines).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}
