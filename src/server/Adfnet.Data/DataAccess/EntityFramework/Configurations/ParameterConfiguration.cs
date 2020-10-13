using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı Parameter tablosu konfigürasyonu
    /// </summary>
    internal class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {

            builder.ToTable("Parameters");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_ParameterDisplayOrder");
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.ParametersCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.ParametersLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.Key).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Key).IsUnique().HasName("UK_ParameterKey");
            builder.Property(x => x.Value).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");
            builder.Property(x => x.Erasable).IsRequired();
            builder.HasOne(x => x.ParameterGroup).WithMany(y => y.Parameters).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }
}