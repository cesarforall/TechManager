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
        private readonly IServiceProvider _serviceProvider;

        private ObservableCollection<Actualizacion> _actualizaciones = new();
        private bool _pendientesChecked = false;

        public RelayCommand OpenCreateActualizacionViewCommand => new(execute => OpenCreateActualizacionView());
        public RelayCommand OpenVerificacionListViewCommand => new(execute => OpenVerificacionListView(execute));
        public RelayCommand ShowActualizacionesPendientesCommand;

        public ActualizacionesViewModel(IActualizacionService actualizacionService, IServiceProvider serviceProvider)
        {
            _actualizacionService = actualizacionService;
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
    }
}
