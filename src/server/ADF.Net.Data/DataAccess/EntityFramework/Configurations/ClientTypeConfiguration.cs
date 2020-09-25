using ADF.Net.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADF.Net.Data.DataAccess.EntityFramework.Configurations
{
    internal class ClientTypeConfiguration : IEntityTypeConfiguration<ClientType>
    {
        public void Configure(EntityTypeBuilder<ClientType> builder)
        {

            builder.ToTable("ClientTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");

            builder.HasIndex(x => x.Code).IsUnique().HasName($"UK_{nameof(ClientType)}Code");

            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");

            builder.Property(x => x.Description).HasColumnType("varchar(512)");

            builder.Property(x => x.CreationTime).IsRequired();

            builder.Property(x => x.LastModificationTime).IsRequired();

        }
    }
}
