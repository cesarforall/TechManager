using Core.Interfaces;
using Core.Services;
using Data;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UI.ViewModels;
using UI.Views;

namespace TechManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "Data Source=techmanager.db";

            // Data
            services.AddTransient<ITecnicoRepository>(provider => new TecnicoRepository(connectionString));

            // Core
            // Cada vez que se solicite ITecnicoService, se creará una nueva instancia de TecnicoService
            services.AddTransient<ITecnicoService, TecnicoService>();

            // UI
            services.AddTransient<UpdateTecnicoViewModel>();
            services.AddTransient<UpdateTecnicoView>();

            services.AddTransient<TecnicosViewModel>();
            services.AddTransient<TecnicosView>();

            services.AddTransient<CreateTecnicoViewModel>();
            services.AddTransient<CreateTecnicoView>();

            services.AddSingleton<MainWindow>();
        }
    }

}
