﻿using System;
using System.IO;
using Adfnet.Core;
using Adfnet.Core.Globalization;
using Adfnet.Data.DataAccess.EntityFramework;
using Adfnet.Setup.Installations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adfnet.Setup
{
    internal class Program
    {
        private static string AppPath
        {
            get
            {
                var path = AppContext.BaseDirectory;
                if (AppContext.BaseDirectory.Contains("bin"))
                {
                    path = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                }
                return path;
            }
        }

        private static IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(AppPath).AddJsonFile("appsettings.json", false, true).Build();


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

                //case "MySqlConnection":
                //    services.AddDbContext<EfDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));
                //    break;
              
                //case "PostgreSqlConnection":
                //    services.AddDbContext<EfDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));
                //    break;

                case "SqliteConnection":
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection").Replace("Data Source=", "Data Source=" + AppPath + "\\")));
                    break;

                default:
                    services.AddDbContext<EfDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("SqliteConnection").Replace("Data Source=", "Data Source=" + AppPath + "\\")));
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
            
            Console.WriteLine(Messages.InfoStartingInstallation);
            Console.WriteLine(Dictionary.StartTime + @": " + DateTime.Now);

            Console.WriteLine(Dictionary.DatabaseType + @": " + Configuration["DefaultConnectionString"].Replace("Connection", ""));
            Console.WriteLine(Dictionary.Server + @": " + dbServer);
            Console.WriteLine(Dictionary.Database + @": " + dbName);
            Console.WriteLine(@"");
            Console.WriteLine(Messages.InfoExistingDatabaseRemoving);

            if (unitOfWork.Context.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                if (unitOfWork.Context.Database.EnsureDeleted())
                {
                    Console.WriteLine(Messages.InfoExistingDatabaseRemoved);
                    Console.WriteLine(@"");
                }
                else
                {
                    Console.WriteLine(Messages.DangerExistingDatabaseNotRemoved);
                    Console.WriteLine(@"");
                    return;
                }
            }

            Console.WriteLine(Messages.InfoNewDatabaseCreating);

            if (unitOfWork.Context.Database.EnsureCreated())
            {

                Console.WriteLine(Messages.InfoNewDatabaseCreated);
                Console.WriteLine(@"");

                try
                {

                    UserInstallation.Install(provider);
                    ParameterInstallation.Install(provider);
                    RoleInstallation.Install(provider);
                    PermissionInstallation.Install(provider);
                    MenuInstallation.Install(provider);
                    LanguageInstallation.Install(provider);
                    CategoryInstallation.Install(provider);


                    if (Configuration["DefaultConnectionString"] == "SqliteConnection")
                    {
                        var setupProjectRootPath = AppContext.BaseDirectory;
                        if (AppContext.BaseDirectory.Contains("bin"))
                        {
                            setupProjectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                        }

                        if (File.Exists(Path.Combine(setupProjectRootPath, dbServer)))
                        {
                            var apiProjectRootPath = setupProjectRootPath.Replace("Adfnet.Setup", "Adfnet.Web.Api");
                            File.Delete(Path.Combine(apiProjectRootPath, dbServer));
                            File.Copy(Path.Combine(setupProjectRootPath, dbServer), Path.Combine(apiProjectRootPath, dbServer));
                        }
                    }



                    Console.WriteLine(Messages.SuccessInstallationOk);
                    Console.WriteLine(Dictionary.EndTime + @": " + DateTime.Now);
                    Console.WriteLine(Messages.InfoCanCloseWindow);
                    Console.WriteLine(@"--------------------------");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine(@"");
                    Console.WriteLine(Dictionary.Error);
                    Console.WriteLine(@"--------------------------");
                }

            }
            else
            {
                Console.WriteLine(Messages.DangerNewDatabaseNotCreated);
                Console.WriteLine(@"");
            }

            //Console.ReadLine();
        }



    }
}
