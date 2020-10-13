using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı MenuHistory tablosu konfigürasyonu
    /// </summary>
    internal class MenuHistoryConfiguration : IEntityTypeConfiguration<MenuHistory>
    {
        public void Configure(EntityTypeBuilder<MenuHistory> builder)
        {

            builder.ToTable("MenuHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();


            builder.Property(x => x.ParentMenuId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Address).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Icon).HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");


        }
    }
}
