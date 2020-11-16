using System;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Adfnet.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.SQLite(Directory.GetCurrentDirectory() + Configuration.GetSection("Logging:LogSqlitePath").Value)
            //    .WriteTo.Seq(Configuration.GetSection("Logging:SeqUrl").Value)
            //    .CreateLogger();

            //Log.Information("Adfnet Baþlatýldý...");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(builder => builder
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Information)
            );

            var origins = Configuration.GetSection("JwtUrl").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                );
            });

            //switch (Configuration["DefaultConnectionString"])
            //{
            //    //case "MsSqlConnection":
            //    //    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));
            //    //    break;

            //    //case "MsSqlLocalDbConnection":
            //    //    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlLocalDbConnection")));
            //    //    break;

            //    //case "MySqlConnection":
            //    //    services.AddDbContext<EfDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));
            //    //    break;

            //    //case "PostgreSqlConnection":
            //    //    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));
            //    //    break;

            //    case "SqliteConnection":
            //        services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
            //        break;

            //    default:
                    
            //        break;
            //}
            services.AddDbContext<EfDbContext>(options => options.UseSqlite("Data Source=AdfnetDB.db"));

            services.ResolveDependency(Configuration);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.MapType<Guid>(() => new OpenApiSchema { Type = "string", Format = "uuid" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Adfnet.Web.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Adfnet.Web.Api v1"));

            // Her istek öncesi loglama için çalýþýr
            app.UseMiddleware<SerilogMiddleware>();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();

            // Her istek öncesi güvenlik için çalýþýr
            app.UseMiddleware<SecurityMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
