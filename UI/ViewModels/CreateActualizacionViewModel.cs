using UI.MVVM;
using Core.Interfaces;
using Core.Models;
using System.Collections.ObjectModel;

namespace UI.ViewModels
{
    public class CreateActualizacionViewModel : ViewModelBase
    {
        private readonly IActualizacionService _actualizacionService;
        private readonly IDispositivoService _dispositivoService;
        private readonly IConocimientoService _conocimientoService;
        private readonly IVerificacionService _verificacionService;
        private ObservableCollection<Dispositivo> _dispositivos = new();
        private Dispositivo _selectedDispositivo = new();
        private string _version = string.Empty;
        private string _descripcion = string.Empty;
        private DateTime? _currentDate;
        private string _message = string.Empty;
        private string _messageColor = "black";

        public RelayCommand CancelNuevaActualizacionCommand => new(execute => CancelNuevaActualizacion());
        public RelayCommand SaveNuevaActualizacionCommand => new(async execute => await SaveNuevaActualizacionAsync());

        public EventHandler? RequestClose;

        public CreateActualizacionViewModel(
            IActualizacionService actualizacionService,
            IDispositivoService dispositivoService,
            IConocimientoService conocimientoService,
            IVerificacionService verificacionService
            )
        {
            _actualizacionService = actualizacionService;
            _dispositivoService = dispositivoService;
            _conocimientoService = conocimientoService;
            _verificacionService = verificacionService;
            CurrentDate = DateTime.Today;
            LoadDispositivosAsync();
        }

        public ObservableCollection<Dispositivo> Dispositivos
        {
            get { return _dispositivos; }
            set { _dispositivos = value; OnPropertyChanged(); }
        }

        public Dispositivo SelectedDispositivo
        {
            get { return _selectedDispositivo; }
            set
            {
                _selectedDispositivo = value;
                Message = string.Empty;
                OnPropertyChanged();
            }
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

        public DateTime? CurrentDate
        {
            get { return _currentDate; }
            set { _currentDate = value; OnPropertyChanged(); }
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

        public async void LoadDispositivosAsync()
        {
            try
            {
                var (succes, message, dispositivos) = await _dispositivoService.GetAll();

                if (succes && dispositivos != null)
                {
                    foreach (var dispositivo in dispositivos)
                    {
                        Dispositivos.Add(new Dispositivo
                        {
                            Id = dispositivo.Id,
                            Fabricante = dispositivo.Fabricante,
                            Modelo = dispositivo.Modelo
                        });
                    }
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al cargar la lista de dispositivos.";
            }
        }

        public async Task SaveNuevaActualizacionAsync()
        {
            if (SelectedDispositivo == null || SelectedDispositivo.Id == 0)
            {
                MessageColor = "red";
                Message = "Seleccione un dispositivo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                MessageColor = "red";
                Message = "Ingrese una versión.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                MessageColor = "red";
                Message = "Ingrese una descripción.";
                return;
            }

            if (CurrentDate == null)
            {
                MessageColor = "red";
                Message = "Seleccione una fecha.";
                return;
            }

            try
            {
                var actualizacion = new Actualizacion
                {
                    DispositivoId = SelectedDispositivo.Id ?? 0,
                    Version = Version,
                    Descripcion = Descripcion,
                    Fecha = CurrentDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var (success, message, id) = await _actualizacionService.Create(actualizacion);

                if (success && id.HasValue)
                {
                    var (conocimientosSuccess, conocimientosMessage, conocimientos) = await _conocimientoService.GetByDispositivoId(SelectedDispositivo.Id ?? 0);

                    if (conocimientosSuccess && conocimientos.Count > 0)
                    {
                        foreach (var conocimiento in conocimientos)
                        {
                            var verificacion = new Verificacion
                            {
                                ActualizacionId = id.Value,
                                TecnicoId = conocimiento.TecnicoId,
                                Confirmado = 0,
                                FechaConfirmacion = string.Empty
                            };

                            await _verificacionService.Create(verificacion);
                        }
                    }

                    MessageColor = "black";
                    Message = "Actualización creada correctamente.";
                }
                else
                {
                    MessageColor = "red";
                    Message = message;
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al crear la actualización.";
            }
        }

        public void CancelNuevaActualizacion()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
