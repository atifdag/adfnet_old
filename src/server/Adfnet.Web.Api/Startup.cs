using System;
using System.IO;
using Adfnet.Data.DataAccess.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Adfnet.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.SQLite(Directory.GetCurrentDirectory() + Configuration.GetSection("Logging:LogSqlitePath").Value)
                .WriteTo.Seq(Configuration.GetSection("Logging:SeqUrl").Value)
                .CreateLogger();

            Log.Information("Adfnet Baþlatýldý...");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

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

            switch (Configuration["DefaultConnectionString"])
            {
                case "MsSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection")));
                    break;

                case "MsSqlLocalDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlLocalDbConnection")));
                    break;

                case "MySqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));
                    break;
                case "MariaDbConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MariaDbConnection")));
                    break;

                case "PostgreSqlConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));
                    break;

                case "SqliteConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
                    break;

                default:
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection")));
                    break;
            }

            services.ResolveDependency(Configuration);
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.MapType<Guid>(() => new OpenApiSchema { Type = "string", Format = "uuid" });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Adfnet API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Her istek öncesi loglama için çalýþýr
            app.UseMiddleware<SerilogMiddleware>();

            app.UseCors("CorsPolicy");

            // Her istek öncesi güvenlik için çalýþýr
            //app.UseMiddleware<SecurityMiddleware>();


            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Adfnet API v1");

            });

            app.UseRouting();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
