using Core.Interfaces;
using Core.Services;
using Data;
using Data.Repositories;
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
            services.AddTransient<IDispositivoRepository>(provider => new DispositivoRepository(connectionString));
            services.AddTransient<IConocimientoRepository>(provider => new ConocimientoRepository(connectionString));

            // Core
            // Cada vez que se solicite ITecnicoService, se creará una nueva instancia de TecnicoService
            services.AddTransient<ITecnicoService, TecnicoService>();
            services.AddTransient<IDispositivoService, DispositivoService>();
            services.AddSingleton<IConocimientoService, ConocimientoService>();

            // UI
            services.AddTransient<UpdateTecnicoViewModel>();
            services.AddTransient<UpdateTecnicoView>();

            services.AddTransient<TecnicosViewModel>();
            services.AddTransient<TecnicosView>();

            services.AddTransient<CreateTecnicoViewModel>();
            services.AddTransient<CreateTecnicoView>();

            // UI - Dispositivos
            services.AddTransient<UpdateDispositivoView>();
            services.AddTransient<UpdateDispositivoViewModel>();

            services.AddTransient<DispositivosView>();
            services.AddTransient<DispositivosViewModel>();

            services.AddTransient<CreateDispositivoViewModel>();
            services.AddTransient<CreateDispositivoView>();

            // UI - Conocimientos
            services.AddTransient<CreateConocimientoViewModel>();
            services.AddTransient<CreateConocimientoView>();

            services.AddTransient<ConocimientoViewModel>();
            services.AddTransient<ConocimientosView>();

            services.AddSingleton<MainWindow>();
        }
    }

}
