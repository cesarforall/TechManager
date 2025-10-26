using Microsoft.Extensions.DependencyInjection;
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
        }

        private void OpenTecnicosView(object sender, RoutedEventArgs e)
        {
            MainContainer.Content = _serviceProvider.GetRequiredService<TecnicosView>();
        }
    }
}