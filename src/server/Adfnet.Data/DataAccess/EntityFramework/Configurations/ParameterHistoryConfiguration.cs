using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı ParameterHistory tablosu konfigürasyonu
    /// </summary>
    internal class ParameterHistoryConfiguration : IEntityTypeConfiguration<ParameterHistory>
    {
        public void Configure(EntityTypeBuilder<ParameterHistory> builder)
        {

            builder.ToTable("ParameterHistories");

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


            builder.Property(x => x.Key).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Value).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");
            builder.Property(x => x.Erasable).IsRequired();
            builder.Property(x => x.ParameterGroupId).IsRequired();
        }
    }
}
