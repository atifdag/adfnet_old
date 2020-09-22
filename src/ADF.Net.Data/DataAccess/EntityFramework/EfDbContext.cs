using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ADF.Net.Data.DataAccess.EntityFramework
{
    public class EfDbContext : DbContext, IDbContext
    {
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

        public EfDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var entityTypeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type => !string.IsNullOrEmpty(type.Namespace) && type.GetInterfaces().Select(x => x.Name).FirstOrDefault() == typeof(IEntityTypeConfiguration<>).Name);

            foreach (var entityTypeConfiguration in entityTypeConfigurations)
            {

                dynamic configurationInstance = Activator.CreateInstance(entityTypeConfiguration);

                modelBuilder.ApplyConfiguration(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

    }
}
