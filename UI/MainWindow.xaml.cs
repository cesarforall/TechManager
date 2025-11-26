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
        private readonly string _version;

        public MainWindow(IServiceProvider serviceProvicer)
        {
            _serviceProvider = serviceProvicer;
            InitializeComponent();

            // Lee la versión dentro de csproj
            _version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.Split('+')[0];
            Title = $"TechManager v{_version}";
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

        private void OpenActualizacionesView(object sender, RoutedEventArgs e)
        {
            MainContainer.Content = _serviceProvider.GetRequiredService<ActualizacionesView>();
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                $"TechManager v{_version}\n\n"
                + "Sistema de gestión de conocimientos técnicos y actualizaciones de dispositivos para laboratorios de reparación\n\n"
                + "Desarrollado por: César Almeida\n"
                + "Proyecto final DAM 2024/2025\n\n"
                + "📁 GitHub: https://github.com/cesarforall/TechManager",
                "Acerca de TechManager",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }
    }
}