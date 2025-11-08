using Core.Interfaces;
using Core.Models;
using UI.MVVM;

namespace UI.ViewModels
{
    public class CreateDispositivoViewModel : ViewModelBase
    {
        private readonly IDispositivoService _dispositivoService;

        private string _fabricante = string.Empty ;
        private string _modelo = string.Empty;
        private string _message = string.Empty;
        private string _messageColor = "black";

        public event EventHandler RequestClose;

        public RelayCommand CancelNuevoDispositivoCommand => new RelayCommand(execute => CancelNuevoDispositivo());
        public RelayCommand SaveNuevoDispositivoCommand => new RelayCommand(execute => SaveNuevoDispositivo());

        public CreateDispositivoViewModel(IDispositivoService dispositivoService)
        {
            _dispositivoService = dispositivoService;
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

        private async void SaveNuevoDispositivo()
        {
            try
            {
                Dispositivo newDispositivo = new Dispositivo
                {
                    Fabricante = this.Fabricante,
                    Modelo = this.Modelo
                };

                var (success, message, id) = await _dispositivoService.Create(newDispositivo);

                Message = message;
                MessageColor = success ? "black" : "red";
            }
            catch (Exception)
            {
                Message = "Ha ocurrido un error al crear el técnico.";
                MessageColor = "red";
            }
        }

        private void CancelNuevoDispositivo()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
