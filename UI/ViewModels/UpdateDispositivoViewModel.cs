using Core.Interfaces;
using Core.Models;
using System.Threading.Tasks;
using UI.MVVM;

namespace UI.ViewModels
{
    public class UpdateDispositivoViewModel : ViewModelBase
    {
        private readonly IDispositivoService _dispositivoService;
        private Dispositivo? _dispositivo;
        private int _id = 0;
        private string _fabricante = string.Empty;
        private string _modelo = string.Empty;
        private string _message = string.Empty;
        private string _messageColor = string.Empty;

        public RelayCommand CancelEditarDispositivoCommand => new(execute => CancelEditarDispositivo());
        public RelayCommand SaveEditarDispositivoCommand => new(async execute => await SaveEditarDispositivo());

        public event EventHandler? RequestClose; 

        public UpdateDispositivoViewModel(IDispositivoService dispositivoService)
        {
            _dispositivoService = dispositivoService;
        }

        public void Initialize(Dispositivo dispositivo)
        {
            _dispositivo = dispositivo;
            _id = dispositivo.Id ?? _id;
            _fabricante = dispositivo.Fabricante ?? _fabricante;
            _modelo = dispositivo.Modelo ?? _modelo;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string Fabricante
        {
            get { return _fabricante; }
            set { _fabricante = value; OnPropertyChanged(); }
        }

        public string Modelo
        {
            get { return _modelo; }
            set { _modelo = value; OnPropertyChanged(); }
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

        private async Task SaveEditarDispositivo()
        {
            try
            {
                Dispositivo updatedDispositivo = new Dispositivo
                {
                    Id = _dispositivo?.Id,
                    Fabricante = this.Fabricante,
                    Modelo = this.Modelo
                };

                var (success, message) = await _dispositivoService.Update(updatedDispositivo);

                MessageColor = success ? "black" : "red";
                Message = message;

                if (success)
                {
                    _dispositivo.Fabricante = updatedDispositivo.Fabricante;
                    _dispositivo.Modelo = updatedDispositivo.Modelo;
                }
            }
            catch (Exception)
            {
                MessageColor = "red";
                Message = "Error al editar el dispositivo.";
            }
        }

        private void CancelEditarDispositivo()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
