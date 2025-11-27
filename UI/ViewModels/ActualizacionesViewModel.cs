using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class ActualizacionesViewModel : ViewModelBase
    {
        private readonly IActualizacionService _actualizacionService;
        private readonly IVerificacionService _verificacionService;
        private readonly IServiceProvider _serviceProvider;

        private ObservableCollection<Actualizacion> _actualizaciones = new();
        private bool _pendientesChecked = false;

        public RelayCommand OpenCreateActualizacionViewCommand => new(execute => OpenCreateActualizacionView());
        public RelayCommand OpenVerificacionListViewCommand => new(execute => OpenVerificacionListView(execute));
        public RelayCommand ShowActualizacionesPendientesCommand;

        public ActualizacionesViewModel(IActualizacionService actualizacionService, IVerificacionService verificacionService, IServiceProvider serviceProvider)
        {
            _actualizacionService = actualizacionService;
            _actualizacionService.ActualizacionCreated += OnActualizacionCreated;
            _verificacionService = verificacionService;
            _verificacionService.VerificacionConfirmed += OnVerificacionConfirmed;
            _serviceProvider = serviceProvider;
            LoadActualizacionesAsync();
        }

        public ObservableCollection<Actualizacion> Actualizaciones
        {
            get { return _actualizaciones; }
            set { _actualizaciones = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Actualizacion> DisplayedActualizaciones
        {
            get
            {
                return PendientesChecked
                    ? new ObservableCollection<Actualizacion>(Actualizaciones.Where(actualizacion => actualizacion.Pendientes > 0))
                    : Actualizaciones;
            }
        }

        public bool PendientesChecked
        {
            get { return _pendientesChecked; }
            set
            {
                _pendientesChecked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayedActualizaciones));
            }
        }

        public async void LoadActualizacionesAsync()
        {
            var (success, message, actualizaciones) = await _actualizacionService.GetAll();

            if (success && actualizaciones != null)
            {
                foreach (var actualizacion in actualizaciones)
                {
                    Actualizaciones.Add(actualizacion);
                }
            }
        }

        public void OpenCreateActualizacionView()
        {
            var createActualizacionView = _serviceProvider.GetRequiredService<CreateActualizacionView>();
            createActualizacionView?.Show();
        }

        private void OpenVerificacionListView(object parameter)
        {
            if (parameter is Actualizacion actualizacion)
            {
                var verificacionListView = _serviceProvider.GetRequiredService<VerificacionesListView>();
                var verificacionListViewModel = verificacionListView.DataContext as VerificacionesListViewModel;
                verificacionListViewModel?.Initialize(actualizacion);
                verificacionListView.Show();
            }
        }

        private async void OnActualizacionCreated(object? sender, int actualizacionId)
        {
            await Task.Delay(50);

            var result = await _actualizacionService.GetById(actualizacionId);
            if (result.success && result.actualizacion != null)
            {
                _actualizaciones.Add(result.actualizacion);
                OnPropertyChanged(nameof(DisplayedActualizaciones));
            }
        }

        private async void OnVerificacionConfirmed(object? sender, int actualizacionId)
        {
            var result = await _actualizacionService.GetById(actualizacionId);
            if (result.success && result.actualizacion != null)
            {
                var existingActualizacion = Actualizaciones.FirstOrDefault(a => a.Id == actualizacionId);
                if (existingActualizacion != null)
                {
                    var index = Actualizaciones.IndexOf(existingActualizacion);

                    Actualizaciones.RemoveAt(index);
                    Actualizaciones.Insert(index, result.actualizacion);

                    OnPropertyChanged(nameof(Actualizaciones));
                    OnPropertyChanged(nameof(DisplayedActualizaciones));
                }
            }
        }
    }
}
