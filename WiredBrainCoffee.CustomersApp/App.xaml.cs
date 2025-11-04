using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using WiredBrainCoffee.CustomersApp.Configuration;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;
using WiredBrainCoffee.CustomersApp.Repository;
using WiredBrainCoffee.CustomersApp.ViewModel;

namespace WiredBrainCoffee.CustomersApp
{
    public partial class App : Application
    {
        private readonly IHost _host;

        // Example I used for using generic host setup:
        // https://laurentkempe.com/2019/09/03/WPF-and-dotnet-Generic-Host-with-dotnet-Core-3-0/
        // https://github.com/laurentkempe/WpfGenericHost/blob/master/App.xaml.cs
        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // DbContext
                    services.AddDbContext<DataContext>(options =>
                        options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")));
                    
                    // Configuration for JSON repositories
                    services.Configure<RepoConfig>(context.Configuration.GetSection("Repository"));

                    // ViewModels
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<CustomersViewModel>();
                    services.AddTransient<ProductsViewModel>();

                    // Repositories
                    services.AddTransient<IRepository<Customer>, CustomerEfRepository>();
                    services.AddTransient<IRepository<Product>, ProductEfRepository>();

                    // Window
                    services.AddTransient<MainWindow>();

                    // JsonSerializerOptions passed to all JSON repos
                    services.AddSingleton(new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.Never
                    });
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow?.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }
            base.OnExit(e);
        }
    }
}
