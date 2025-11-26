using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
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
        public RelayCommand DeleteDispositivoCommand => new(execute => DeleteDispositivo(execute));

        public RelayCommand OpenEditDispositivoViewCommand => new(execute => OpenEditTecnicoView(execute));

        public DispositivosViewModel(IDispositivoService dispositivoService, IServiceProvider serviceProvider)
        {
            _dispositivoService = dispositivoService;
            _dispositivoService.DispositivoCreated += OnDispositivoCreated;
            _dispositivoService.DispositivoUpdated += OnDispositivoUpdated;
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

        private void OpenEditTecnicoView(object parameter)
        {
            if (parameter is Dispositivo dispositivo)
            {
                var updateDispositivoView = _serviceProvider.GetRequiredService<UpdateDispositivoView>();
                var updateDispositivoViewModel = updateDispositivoView.DataContext as UpdateDispositivoViewModel;
                updateDispositivoViewModel?.Initialize(dispositivo);
                updateDispositivoView.Show();
            }
        }

        private async void DeleteDispositivo(object parameter)
        {
            if (parameter is Dispositivo dispositivo && dispositivo.Id is int id)
            {
                if (MessageBox.Show($"¿Desea eliminar el dispositivo con ID: {id}?\n\n- Perderá todos los datos asociados al dispositivo.",
                    "Eliminar dispositivo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var result = await _dispositivoService.Delete(id);
                    if (result.success)
                    {
                        Dispositivos.Remove(dispositivo);
                    }
                }
            }
        }

        private async void OnDispositivoCreated(object? sender, int dispositivoId)
        {
            var result = await _dispositivoService.GetById(dispositivoId);
            if (result.success && result.dispositivo != null)
            {
                Dispositivos.Add(result.dispositivo);
            }
        }

        private async void OnDispositivoUpdated(object? sender, int dispositivoId)
        {
            var result = await _dispositivoService.GetById(dispositivoId);
            if (result.success && result.dispositivo != null)
            {
                var existingDispositivo = Dispositivos.FirstOrDefault(d => d.Id == dispositivoId);
                if (existingDispositivo != null)
                {
                    var index = Dispositivos.IndexOf(existingDispositivo);
                    Dispositivos.RemoveAt(index);
                    Dispositivos.Insert(index, result.dispositivo);
                }
            }
        }
    }
}
