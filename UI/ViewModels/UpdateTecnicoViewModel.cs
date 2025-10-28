using Core.Interfaces;
using Core.Models;
using UI.MVVM;

namespace UI.ViewModels
{
    public class UpdateTecnicoViewModel : ViewModelBase
    {
        private readonly ITecnicoService _tecnicoService;

        private int _id;
        private string? _nombre;
        private string? _apellidos;
        private int? _gaveta;
        private string? _nombrePC;
        private string? _usuarioPC;
        private string _message;
        private string _messageColor;

        public RelayCommand CancelEditarTecnicoCommand => new RelayCommand(execute => CancelEditarTecnico());
        public RelayCommand SaveEditarTecnicoCommand => new RelayCommand(async execute => await UpdateTecnicoAsync());

        // Evento para solicitar a la vista que se cierre
        public event EventHandler? RequestClose;

        public UpdateTecnicoViewModel(ITecnicoService tecnicoService)
        {
            _tecnicoService = tecnicoService;
        }
        public void Initialize(Tecnico tecnico)
        {
            Id = tecnico.Id;
            Nombre = tecnico.Nombre;
            Apellidos = tecnico.Apellidos;
            Gaveta = tecnico.Gaveta;
            NombrePC = tecnico.NombrePC;
            UsuarioPC = tecnico.UsuarioPC;
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

        public int Id
        {   
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string? Nombre
        {   
            get { return _nombre; }
            set { _nombre = value; OnPropertyChanged(); }
        }

        public string? Apellidos
        {   
            get { return _apellidos; }
            set { _apellidos = value; OnPropertyChanged(); }
        }

        public int? Gaveta
        {   
            get { return _gaveta; }
            set { _gaveta = value; OnPropertyChanged(); }
        }

        public string? NombrePC
        {   
            get { return _nombrePC; }
            set { _nombrePC = value; OnPropertyChanged(); }
        }

        public string? UsuarioPC
        {   
            get { return _usuarioPC; }
            set { _usuarioPC = value; OnPropertyChanged(); }
        }

        private async Task UpdateTecnicoAsync()
        {
            Message = string.Empty;
            MessageColor = "Black";

            try
            {
                var updatedTecnico = new Tecnico
                {
                    Id = Id,
                    Nombre = Nombre,
                    Apellidos = Apellidos,
                    Gaveta = Gaveta,
                    NombrePC = NombrePC,
                    UsuarioPC = UsuarioPC
                };
                
                var result = await _tecnicoService.update(updatedTecnico);
                Message = result.message;

                if (!result.success)
                {
                    MessageColor = "Red";
                }
            }
            catch (Exception)
            {

                MessageColor = "Red";
                Message = "Ha ocurrido un error al actualizar el técnico.";
            }
        }

        private void CancelEditarTecnico()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

    }
}
