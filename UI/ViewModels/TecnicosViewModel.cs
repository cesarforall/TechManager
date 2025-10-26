using Core.Interfaces;
using Core.Models;
using System.Collections.ObjectModel;
using UI.MVVM;

namespace UI.ViewModels
{
    public class TecnicosViewModel : ViewModelBase
    {
        private readonly ITecnicoService _tecnicoService;
        ObservableCollection<Tecnico> _tecnicos;

        public TecnicosViewModel(ITecnicoService tecnicoService)
        {
            _tecnicoService = tecnicoService;
            _tecnicos = new ObservableCollection<Tecnico>();
            InitializeAsync();
        }

        public ObservableCollection<Tecnico> Tecnicos
        {
            get { return _tecnicos; }
            set
            {
                _tecnicos = value;
                OnPropertyChanged();
            }
        }

        // Cargar los técnicos de forma asíncrona al inicializar el ViewModel
        public async void InitializeAsync()
        {
            await LoadTecnicosAsync();
        }

        public async Task LoadTecnicosAsync() {
            var result = await _tecnicoService.getAll();

            if (result.success && result.tecnicos != null)
            {
                Tecnicos = new ObservableCollection<Tecnico>(result.tecnicos);
            }
        }
    }
}
