using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný RoleUserLine tablosu konfigürasyonu
    /// </summary>
    internal class RoleUserLineConfiguration : IEntityTypeConfiguration<RoleUserLine>
    {
        public void Configure(EntityTypeBuilder<RoleUserLine> builder)
        {
            builder.ToTable("RoleUserLines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_RoleUserLineDisplayOrder");
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.RoleUserLinesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.RoleUserLinesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Role).WithMany(y => y.RoleUserLines).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.User).WithMany(y => y.RoleUserLines).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}
