using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný RolePermissionLine tablosu konfigürasyonu
    /// </summary>
    internal class RolePermissionLineConfiguration : IEntityTypeConfiguration<RolePermissionLine>
    {
        public void Configure(EntityTypeBuilder<RolePermissionLine> builder)
        {
            builder.ToTable("RolePermissionLines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_RolePermissionLineDisplayOrder");
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.RolePermissionLinesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.RolePermissionLinesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Role).WithMany(y => y.RolePermissionLines).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Permission).WithMany(y => y.RolePermissionLines).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}
