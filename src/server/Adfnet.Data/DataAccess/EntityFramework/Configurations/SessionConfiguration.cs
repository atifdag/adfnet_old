using Adfnet.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adfnet.Data.DataAccess.EntityFramework.Configurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.SessionsCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);

        }
    }
}