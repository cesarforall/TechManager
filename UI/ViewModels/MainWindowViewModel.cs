using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private object _currentView;
        private string _title;

        public RelayCommand OpenTecnicosViewCommand => new RelayCommand(execute => OpenTecnicosView());
        public RelayCommand OpenDispositivosViewCommand => new RelayCommand(execute => OpenDispositivosView());
        public RelayCommand OpenConocimientosViewCommand => new RelayCommand(execute => OpenConocimientosView());
        public RelayCommand OpenActualizacionesViewCommand => new RelayCommand(execute => OpenActualizacionesView());
        public RelayCommand ShowAboutCommand => new RelayCommand(execute => ShowAbout());

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Lee la versión dentro de csproj
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion?.Split('+')[0];
            Title = $"TechManager v{version}";
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private void OpenTecnicosView()
        {
            CurrentView = _serviceProvider.GetRequiredService<TecnicosView>();
        }

        private void OpenDispositivosView()
        {
            CurrentView = _serviceProvider.GetRequiredService<DispositivosView>();
        }

        private void OpenConocimientosView()
        {
            CurrentView = _serviceProvider.GetRequiredService<ConocimientosView>();
        }

        private void OpenActualizacionesView()
        {
            CurrentView = _serviceProvider.GetRequiredService<ActualizacionesView>();
        }

        private void ShowAbout()
        {
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion?.Split('+')[0];
            MessageBox.Show(
                $"TechManager v{version}\n\n"
                + "Sistema de gestión de conocimientos técnicos y actualizaciones de dispositivos para laboratorios de reparación\n\n"
                + "Desarrollado por: César Almeida\n"
                + "Proyecto final DAM 2025/2026\n\n"
                + "📁 GitHub: https://github.com/cesarforall/TechManager",
                "Acerca de TechManager",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}
