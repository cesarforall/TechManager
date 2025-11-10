using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UI.Views;

namespace TechManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvicer)
        {
            _serviceProvider = serviceProvicer;
            InitializeComponent();

            // Lee la versión dentro de csproj
            var fullVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            var cleanVersion = fullVersion.Split('+')[0];
            Title = $"TechManager v{cleanVersion}";
        }

        private void OpenTecnicosView(object sender, RoutedEventArgs e)
        {
            MainContainer.Content = _serviceProvider.GetRequiredService<TecnicosView>();
        }

        private void OpenDispositivosView(object sender, RoutedEventArgs e)
        {
            MainContainer.Content = _serviceProvider.GetRequiredService<DispositivosView>();
        }

        private void OpenConocimientosView(object sender, RoutedEventArgs e)
        {
            MainContainer.Content = _serviceProvider.GetRequiredService<ConocimientosView>();
        }
    }
}