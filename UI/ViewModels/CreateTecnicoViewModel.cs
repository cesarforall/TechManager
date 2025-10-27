using Core.Interfaces;
using Core.Models;
using UI.MVVM;

namespace UI.ViewModels
{
    public class CreateTecnicoViewModel: ViewModelBase
    {
		private readonly ITecnicoService _tecnicoService;
		public RelayCommand SaveNuevoTecnicoCommand => new RelayCommand(async execute => await SaveNuevoTecnico(Tecnico));
		public RelayCommand CancelNuevoTecnicoCommand => new RelayCommand(execute => CancelNuevoTecnico());

        // Evento para solicitar a la vista que se cierre
        public event EventHandler? RequestClose;

        private Tecnico _tecnico = null!;
        private string _message;
		private string _messageColor;

        public CreateTecnicoViewModel(ITecnicoService tecnicoService)
		{
			_tecnicoService = tecnicoService;
			Tecnico = new Tecnico();
        }

		public Tecnico Tecnico
		{
			get { return _tecnico; }
			set
			{
                _tecnico = value;
				OnPropertyChanged();
            }
		}

		public string MessageColor
		{
			get { return _messageColor; }
			set { _messageColor = value; OnPropertyChanged(); }
		}

		public string Message
		{
			get { return _message; }
            set
			{
				_message = value;
				OnPropertyChanged();
            }
		}

        public async Task SaveNuevoTecnico(Tecnico tecnico)
		{
			Message = string.Empty;
			MessageColor = "Black";

            try
			{
				var result = await _tecnicoService.create(tecnico);
				Message = result.message;
				if (!result.success)
				{
                    MessageColor = "Red";
                }
				if (result.success)
				{
					Tecnico = new Tecnico();
                }

            }
			catch (Exception)
			{
				Message = "Ha ocurrido un error durante la creación del técnico.";
            }			
        }

        private void CancelNuevoTecnico()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
