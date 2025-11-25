using Core.Interfaces;
using Core.Models;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using UI.MVVM;

namespace UI.ViewModels
{
    public class VerificacionesListViewModel : ViewModelBase
    {
        private readonly IVerificacionService _verificacionService;
        private Actualizacion? _actualizacion;
        private int _id;
        private Dispositivo _dispositivo = new();
        private string _version = string.Empty;
        private string _descripcion = string.Empty;
        private string _fecha = string.Empty;
        private ObservableCollection<Verificacion> _verificaciones = new();
        private ObservableCollection<Verificacion> _displayedVerificaciones = new();
        private bool _pendientesChecked;
        private string _message = string.Empty;
        private string _messageColor = "black";

        public RelayCommand CancelVerificacionesCommand => new(execute => CancelVerificaciones());
        public RelayCommand SaveVerificacionesCommand => new(async execute => await SaveVerificacionesAsync());
        public RelayCommand ToggleConfirmadoCommand => new(obj => ToggleConfirmado(obj));

        public EventHandler? RequestClose;

        public VerificacionesListViewModel(IVerificacionService verificacionService)
        {
            _verificacionService = verificacionService;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public Dispositivo Dispositivo
        {
            get { return _dispositivo; }
            set { _dispositivo = value; OnPropertyChanged(); }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; OnPropertyChanged(); }
        }

        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; OnPropertyChanged(); }
        }

        public string Fecha
        {
            get { return _fecha; }
            set { _fecha = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Verificacion> Verificaciones
        {
            get { return _verificaciones; }
            set { _verificaciones = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Verificacion> DisplayedVerificaciones
        {
            get { return _displayedVerificaciones; }
            set { _displayedVerificaciones = value; OnPropertyChanged(); }
        }

        public bool PendientesChecked
        {
            get { return _pendientesChecked; }
            set
            {
                _pendientesChecked = value;
                OnPropertyChanged();
                FilterVerificaciones();
            }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        public string MessageColor
        {
            get { return _messageColor; }
            set { _messageColor = value; OnPropertyChanged(); }
        }

        public async void Initialize(Actualizacion actualizacion)
        {
            try
            {
                _actualizacion = actualizacion;
                Id = actualizacion.Id;
                Dispositivo = actualizacion.Dispositivo ?? new Dispositivo();
                Version = actualizacion.Version ?? string.Empty;
                Descripcion = actualizacion.Descripcion ?? string.Empty;
                Fecha = actualizacion.Fecha ?? string.Empty;

                await LoadVerificacionesAsync(actualizacion.Id);
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al inicializar la vista.";
            }
        }

        public async Task LoadVerificacionesAsync(int actualizacionId)
        {
            try
            {
                var (success, message, verificaciones) = await _verificacionService.GetByActualizacionId(actualizacionId);

                if (success && verificaciones != null)
                {
                    Verificaciones.Clear();
                    DisplayedVerificaciones.Clear();

                    foreach (var verificacion in verificaciones)
                    {
                        Verificaciones.Add(verificacion);
                        DisplayedVerificaciones.Add(verificacion);
                    }
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al cargar las verificaciones.";
            }
        }

        public void FilterVerificaciones()
        {
            DisplayedVerificaciones.Clear();

            if (PendientesChecked)
            {
                foreach (var verificacion in Verificaciones.Where(v => v.Confirmado == 0))
                {
                    DisplayedVerificaciones.Add(verificacion);
                }
            }
            else
            {
                foreach (var verificacion in Verificaciones)
                {
                    DisplayedVerificaciones.Add(verificacion);
                }
            }
        }

        public async Task SaveVerificacionesAsync()
        {
            try
            {
                bool hayConfirmaciones = false;

                foreach (var verificacion in Verificaciones)
                {
                    if (verificacion.Confirmado == 1 && string.IsNullOrEmpty(verificacion.FechaConfirmacion))
                    {
                        var (success, message) = await _verificacionService.ConfirmVerification(
                            verificacion.ActualizacionId,
                            verificacion.TecnicoId);

                        if (!success)
                        {
                            MessageColor = "red";
                            Message = $"Error al confirmar verificación del técnico {verificacion.Tecnico.Id}.";
                            return;
                        }

                        hayConfirmaciones = true;
                    }
                }

                if (hayConfirmaciones)
                {
                    MessageColor = "black";
                    Message = "Verificaciones guardadas correctamente.";
                    await LoadVerificacionesAsync(Id);
                    FilterVerificaciones();
                }
                else
                {
                    MessageColor = "black";
                    Message = "No hay cambios para guardar.";
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al guardar las verificaciones.";
            }
        }

        public void ToggleConfirmado(object obj)
        {
            if (obj is Verificacion verificacion && verificacion.Confirmado == 0)
            {
                verificacion.Confirmado = 1;
            }
        }

        public void CancelVerificaciones()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
