using System;
using ADF.Net.Data;
using ADF.Net.Data.DataAccess.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Installation.ConsoleApp
{
    internal class Program
    {
        private static IConfiguration Configuration
        {
            get
            {

                var path = AppContext.BaseDirectory;
                if (AppContext.BaseDirectory.Contains("bin"))
                {
                    path = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                }
                return new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json", false, true).Build();
            }
        }



        private static void Main()
        {

            var services = new ServiceCollection();

            switch (Configuration["DefaultConnectionString"])
            {

                case "MsSqlAzureConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MsSqlAzureConnection")));
                    break;

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



            services.AddScoped(typeof(IUnitOfWork<EfDbContext>), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            var provider = services.BuildServiceProvider();

            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();

            var dbName = "";
            var dbServer = "";

            foreach (var s in Configuration.GetSection("ConnectionStrings:" + Configuration["DefaultConnectionString"]).Value.Split(";"))
            {
                if (s.Contains("Data Source"))
                {
                    dbServer = s.Replace("Data Source=", "");
                }

                if (s.Contains("Server"))
                {
                    dbServer = s.Replace("Server=", "");
                }

                if (s.Contains("Host"))
                {
                    dbServer = s.Replace("Host=", "");
                }

                if (s.Contains("Database"))
                {
                    dbName = s.Replace("Database=", "");
                }

                if (s.Contains("Initial Catalog"))
                {
                    dbName = s.Replace("Initial Catalog=", "");
                }
            }

            Console.WriteLine(@"Kurulum başlatılıyor...");
            Console.WriteLine(@"Başlama Zamanı: " + DateTime.Now);

            Console.WriteLine(@"Veritabanı Türü: " + Configuration["DefaultConnectionString"].Replace("Connection", ""));
            Console.WriteLine(@"Veritabanı Sunucusu: " + dbServer);
            Console.WriteLine(@"Veritabanı Adı: " + dbName);
            Console.WriteLine(@"");
            Console.WriteLine(@"Mevcut veritabanı kaldırılıyor...");

            if (unitOfWork.Context.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                if (unitOfWork.Context.Database.EnsureDeleted())
                {
                    Console.WriteLine(@"Mevcut veritabanı kaldırıldı.");
                    Console.WriteLine(@"");
                }
                else
                {
                    Console.WriteLine(@"Hata: Mevcut veritabanı kaldırılamadı!");
                    Console.WriteLine(@"");
                    return;
                }
            }

            Console.WriteLine(@"Yeni veritabanı oluşturuluyor...");

            if (unitOfWork.Context.Database.EnsureCreated())
            {

                Console.WriteLine(@"Yeni veritabanı oluşturuldu.");
                Console.WriteLine(@"");

                try
                {
                    ProductInstallation.Install(provider);
                    Console.WriteLine(@"Products installed.");
                    Console.WriteLine(@"");

                    Console.WriteLine(@"Kurulum Tamamlandı.");
                    Console.WriteLine(@"Bitiş Zamanı: " + DateTime.Now);
                    Console.WriteLine(@"Programı kapatabilirsiniz.");
                    Console.WriteLine(@"--------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(@"");
                    Console.WriteLine(@"Hata oluştu.");
                    Console.WriteLine(@"--------------------------");
                }

            }
            else
            {
                Console.WriteLine(@"Hata: Yeni veritabanı oluşturulamadı!");
                Console.WriteLine(@"");
            }

            //Console.ReadLine();
        }



    }
}
