using ADF.Net.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADF.Net.Data.DataAccess.EF.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Code).IsUnique().HasName("UK_ProductCode");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Description).HasColumnType("varchar(512)");

            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.LastModificationTime).IsRequired();
        }
    }
}
