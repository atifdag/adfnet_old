using System.Linq;
using System.Reflection;
using ADF.Net.Core;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using ADF.Net.Service;
using ADF.Net.Service.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Web.Api
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveDependency(this IServiceCollection services, IConfiguration configuration)
        {
           
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

            services.AddHttpContextAccessor();

            services.AddTransient<IMainService, MainService>();

        }
    }
}
