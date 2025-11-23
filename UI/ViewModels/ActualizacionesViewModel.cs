using Core.Interfaces;
using Core.Models;
using System.Collections.ObjectModel;
using UI.MVVM;

namespace UI.ViewModels
{
    public class ActualizacionesViewModel : ViewModelBase
    {
        private readonly IActualizacionService _actualizacionService;

        private ObservableCollection<Actualizacion> _actualizaciones = new();
        private bool _pendientesChecked = false;

        public RelayCommand OpenCreateActualizacionViewCommand;
        public RelayCommand OpenActualizacionViewCommand;
        public RelayCommand ShowActualizacionesPendientesCommand;

        public ActualizacionesViewModel(IActualizacionService actualizacionService)
        {
            _actualizacionService = actualizacionService;
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
    }
}
