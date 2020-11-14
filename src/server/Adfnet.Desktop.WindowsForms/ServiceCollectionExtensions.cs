using System.Linq;
using System.Reflection;
using Adfnet.Core;
using Adfnet.Core.Caching;
using Adfnet.Core.Helpers;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Desktop.WindowsForms.Forms.AuthenticationForms;
using Adfnet.Desktop.WindowsForms.Forms.HomeForms;
using Adfnet.Service;
using Adfnet.Service.Implementations;
using Adfnet.Service.Implementations.EmailMessaging.SystemNet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Desktop.WindowsForms
{
    public static class ServiceCollectionExtensions
    {
        public static void ResolveDependency(this IServiceCollection services, IConfiguration configuration)
        {

            Init(services, configuration);

            var dependencies = Assembly.GetAssembly(typeof(MainService))?.GetTypes().Where(t => t.GetInterfaces().Select(x => x.Name).Contains(nameof(ITransientDependency)));

            if (dependencies == null) return;
            {
                foreach (var dependency in dependencies)
                {
                    var iDependency = dependency.GetInterfaces().FirstOrDefault();
                    services.AddTransient(iDependency, dependency);
                }
            }

            services.AddSingleton<HomeForm>();
            services.AddSingleton<LoginForm>();
            services.AddSingleton<RegisterForm>();
            services.AddSingleton<ForgotPasswordForm>();

        }

        private static void Init(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(provider => configuration);
            services.AddScoped<IDbContext>(provider => provider.GetService<EfDbContext>());

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            services.AddTransient<IMainService, MainService>();

            services.AddTransient<ISmtp, SystemNetSmtp>();

            if (configuration.GetSection("Caching:CacheType").Value == "Memory")
            {
                services.AddTransient<ICacheService>(provider => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), configuration.GetSection("Caching:CacheTime").Value.ToInt()));
            }
            else if (configuration.GetSection("Caching:CacheType").Value == "Redis")
            {
                services.AddTransient<ICacheService>(s => new RedisCacheService(configuration.GetSection("RedisServer:Host").Value, configuration.GetSection("Caching:RedisServer:Port").Value.ToInt(), configuration.GetSection("Caching:CacheTime").Value.ToInt()));
            }

            services.AddTransient<IIdentityService, FileBaseIdentityService>(provider => new FileBaseIdentityService(configuration.GetSection("JwtSecurityKey").Value));

            services.AddTransient<IAuthenticationService, AuthenticationService>();

        }
    }
}
