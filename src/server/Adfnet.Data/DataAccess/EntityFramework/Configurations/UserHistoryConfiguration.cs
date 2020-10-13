using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný UserHistory tablosu konfigürasyonu
    /// </summary>
    internal class UserHistoryConfiguration : IEntityTypeConfiguration<UserHistory>
    {
        public void Configure(EntityTypeBuilder<UserHistory> builder)
        {
            // Tablo adý
            builder.ToTable("UserHistories");

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

            builder.Property(x => x.Username).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.Password).IsRequired().HasColumnType("char(128)");
            builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.PersonId).IsRequired();
            builder.Property(x => x.LanguageId).IsRequired();

        }
    }
}