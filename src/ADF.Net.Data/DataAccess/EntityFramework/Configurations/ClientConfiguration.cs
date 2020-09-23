using ADF.Net.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADF.Net.Data.DataAccess.EntityFramework.Configurations
{
    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {

            builder.ToTable("Clients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(512)");

            builder.HasIndex(x => x.Code).IsUnique().HasName($"UK_{nameof(Client)}Code");

            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(512)");

            builder.Property(x => x.Description).HasColumnType("varchar(512)");

            builder.Property(x => x.Address).HasColumnType("varchar(512)");

            builder.Property(x => x.Phone).HasColumnType("varchar(512)");

            builder.Property(x => x.SpecialCode).HasColumnType("varchar(512)");
            builder.Property(x => x.AuthorizedPerson).HasColumnType("varchar(512)");
            builder.Property(x => x.RelatedPerson).HasColumnType("varchar(512)");
            builder.Property(x => x.RelatedPersonPhone).HasColumnType("varchar(512)");

            builder.Property(x => x.CreationTime).IsRequired();

            builder.Property(x => x.LastModificationTime).IsRequired();

            builder.HasOne(x => x.ClientType).WithMany(y => y.Clients).IsRequired().OnDelete(DeleteBehavior.Restrict);

        }
    }
}