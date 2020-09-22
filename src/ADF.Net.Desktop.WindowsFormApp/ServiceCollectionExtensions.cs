using System.Linq;
using System.Reflection;
using ADF.Net.Core;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Service;
using ADF.Net.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveDependency(this IServiceCollection services, IConfiguration configuration)
        {

            switch (configuration["DefaultConnectionString"])
            {

                case "MsSqlAzureConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlAzureConnection")));
                    break;

                case "MsSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlConnection")));
                    break;

                case "MsSqlLocalDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MsSqlLocalDbConnection")));
                    break;

                case "MySqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySQL(configuration.GetConnectionString("MySqlConnection")));
                    break;
                case "MariaDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySql(configuration.GetConnectionString("MariaDbConnection")));
                    break;

                case "PostgreSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection")));
                    break;

                case "SqliteConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(configuration.GetConnectionString("SqliteConnection")));
                    break;

                default:
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(configuration.GetConnectionString("SqliteConnection")));
                    break;
            }

            Init(services);

            var dependencies = Assembly.GetAssembly(typeof(MainService))?.GetTypes().Where(t => t.GetInterfaces().Select(x => x.Name).Contains(nameof(ITransientDependency)));

            if (dependencies == null) return;
            {
                foreach (var dependency in dependencies)
                {
                    var iDependency = dependency.GetInterfaces().FirstOrDefault();
                    services.AddTransient(iDependency, dependency);
                }
            }

        }

        private static void Init(this IServiceCollection services)
        {


            services.AddScoped<IDbContext>(provider => provider.GetService<EfDbContext>());

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddTransient<IMainService, MainService>();

        }
    }
}
