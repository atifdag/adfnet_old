using System;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Web.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            //Log.Information("Adfnet Ba�lat�ld�...");
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
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Adfnet.Web.Api", Version = "v2" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Her istek �ncesi loglama i�in �al���r
                app.UseMiddleware<SerilogMiddleware>();

                app.UseCors("CorsPolicy");

                // Her istek �ncesi g�venlik i�in �al���r
                app.UseMiddleware<SecurityMiddleware>();


                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Adfnet.Web.Api v2"));
            }

        //    app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
