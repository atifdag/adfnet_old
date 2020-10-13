using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný User tablosu konfigürasyonu
    /// </summary>
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tablo adý
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_UserDisplayOrder");
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.UsersCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.UsersLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Username).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Username).IsUnique().HasName("UK_UserUsername");
            builder.Property(x => x.Password).IsRequired().HasColumnType("char(128)");
            builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(512)");
            builder.HasIndex(x => x.Email).IsUnique().HasName("UK_UserEmail");
            builder.HasOne(x => x.Person).WithMany(y => y.Users).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Language).WithMany(y => y.Users).OnDelete(DeleteBehavior.Restrict);

        }
    }
}