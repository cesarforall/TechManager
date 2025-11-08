using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class DispositivosViewModel : ViewModelBase
    {
        private readonly IDispositivoService _dispositivoService;
        private IServiceProvider _serviceProvider;

        private ObservableCollection<Dispositivo> _dispositivos;

        public RelayCommand OpenCreateDispositivoViewCommand => new(execute => OpenCreateDispositivoView());

        public DispositivosViewModel(IDispositivoService dispositivoService, IServiceProvider serviceProvider)
        {
            _dispositivoService = dispositivoService;
            _serviceProvider = serviceProvider;
            InitializeAsync();
        }

        public ObservableCollection<Dispositivo> Dispositivos
        {
            get { return _dispositivos; }
            set
            {
                _dispositivos = value;
                OnPropertyChanged();
            }
        }

        // Cargar los técnicos de forma asíncrona al inicializar el ViewModel
        public async void InitializeAsync()
        {
            await LoadDispositivosAsync();
        }

        public async Task LoadDispositivosAsync()
        {
            var result = await _dispositivoService.GetAll();

            if (result.success && result.dispositivo != null)
            {
                Dispositivos = new ObservableCollection<Dispositivo>(result.dispositivo);
            }
        }

        public void OpenCreateDispositivoView()
        {
            var createDispositivoView = _serviceProvider.GetService<CreateDispositivoView>();
            createDispositivoView?.Show();
        }
    }
}
