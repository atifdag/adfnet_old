using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{
    internal class SessionHistoryConfiguration : IEntityTypeConfiguration<SessionHistory>
    {
        public void Configure(EntityTypeBuilder<SessionHistory> builder)
        {
            builder.ToTable("SessionHistories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.LogoutType).IsRequired().HasColumnType("varchar(512)");
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.SessionHistoriesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}