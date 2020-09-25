using System;
using System.Windows.Forms;
using ADF.Net.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADF.Net.Desktop.WindowsFormApp
{
    public class Program
    {

        private static ICategoryService _serviceCategory;
        private static IProductService _serviceProduct;

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

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var services = new ServiceCollection();
            services.ResolveDependency(Configuration);

            var provider = services.BuildServiceProvider();
            _serviceCategory = provider.GetService<ICategoryService>();
            _serviceProduct = provider.GetService<IProductService>();
            Application.Run(new MainForm(_serviceCategory, _serviceProduct));
        }
    }
}
