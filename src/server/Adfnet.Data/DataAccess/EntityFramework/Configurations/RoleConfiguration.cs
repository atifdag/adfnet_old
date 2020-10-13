using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı Role tablosu konfigürasyonu
    /// </summary>
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Tablo adı
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_RoleDisplayOrder");
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.RolesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.RolesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Code).IsUnique().HasName("UK_RoleCode");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");
            builder.Property(x => x.Level).IsRequired();

        }
    }
}
