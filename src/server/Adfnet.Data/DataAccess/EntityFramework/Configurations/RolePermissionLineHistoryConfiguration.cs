using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný PersonHistory tablosu konfigürasyonu
    /// </summary>
    internal class RolePermissionLineHistoryConfiguration : IEntityTypeConfiguration<RolePermissionLineHistory>
    {
        public void Configure(EntityTypeBuilder<RolePermissionLineHistory> builder)
        {
            builder.ToTable("RolePermissionLineHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();


            builder.Property(x => x.RoleId).IsRequired();
            builder.Property(x => x.PermissionId).IsRequired();

        }
    }
}
