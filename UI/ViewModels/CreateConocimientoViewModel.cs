using UI.MVVM;
using Core.Interfaces;
using Core.Models;
using System.Collections.ObjectModel;

namespace UI.ViewModels
{
    public class CreateConocimientoViewModel : ViewModelBase
    {
        private readonly IConocimientoService _conocimientoService;
        private readonly ITecnicoService _tecnicoService;
        private ObservableCollection<Tecnico> _tecnicos = new();
        private ObservableCollection<Conocimiento> _availableConocimientos = new();
        private Tecnico _selectedTecnico = new();
        private string _message = string.Empty;
        private string _messageColor = "black";

        public RelayCommand CancelNuevoConocimientoCommand => new(execute => CancelNuevoConocimiento());
        public RelayCommand SaveNuevoConocimientoCommand => new(async execute => await SaveNuevoConocimientoAsync());

        public EventHandler? RequestClose;

        public CreateConocimientoViewModel(IConocimientoService conocimientoService, ITecnicoService tecnicoService)
        {
            _conocimientoService = conocimientoService;
            _tecnicoService = tecnicoService;
            LoadTecnicosAsync();
        }

        public ObservableCollection<Tecnico> Tecnicos
        {
            get { return _tecnicos; }
            set { _tecnicos = value; OnPropertyChanged(); }
        }

        public Tecnico SelectedTecnico
        {
            get { return _selectedTecnico; }
            set
            {
                _selectedTecnico = value;
                LoadAvailableConocimientos();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Conocimiento> AvailableConocimientos
        {
            get { return _availableConocimientos; }
            set { _availableConocimientos = value; OnPropertyChanged(); }
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

        public async void LoadTecnicosAsync()
        {
            try
            {
                var (succes, message, tecnicos) = await _tecnicoService.getAll();

                if (succes && tecnicos != null)
                {
                    foreach (var tecnico in tecnicos)
                    {
                        Tecnicos.Add(new Tecnico
                        {
                            Id = tecnico.Id,
                            Nombre = tecnico.Nombre,
                            Apellidos = tecnico.Apellidos,
                            Gaveta = tecnico.Gaveta,
                            NombrePC = tecnico.NombrePC,
                            UsuarioPC = tecnico.UsuarioPC
                        });
                    }
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al cargar la lista de técnicos.";
            }
        }

        public async void LoadAvailableConocimientos()
        {
            if (SelectedTecnico == null || SelectedTecnico.Id == 0) return;

            AvailableConocimientos.Clear();

            try
            {
                var (succes, message, conocimientos) = await _conocimientoService.GetAvailableConocimientosByTecnicoId(SelectedTecnico.Id);

                if (succes && conocimientos.Count == 0)
                {
                    MessageColor = "black";
                    Message = "El técnico ya tiene todos los dispositivos asociados.";
                }
                if (succes && conocimientos.Count > 0)
                {
                    foreach (var conocimiento in conocimientos)
                    {
                        AvailableConocimientos.Add(conocimiento);
                    }
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al cargar la lista de dispositivos.";
            }
        }

        public async Task SaveNuevoConocimientoAsync()
        {
            if (AvailableConocimientos.Count > 0)
            {
                try
                {
                    foreach (var conocimiento in AvailableConocimientos)
                    {
                        var (success, message, id) = await _conocimientoService.Create(conocimiento);

                        if (!success) { Message = message; MessageColor = "red"; }
                    }

                    MessageColor = "black";
                    Message = "Conocimientos asociados correctamente";
                }
                catch (Exception)
                {
                    MessageColor = "red";
                    Message = "Error al asociar conocimientos.";
                }
            }
            else
            {
                MessageColor = "red";
                Message = "Sin conocimientos seleccionados.";
            }
        }

        public void CancelNuevoConocimiento()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
