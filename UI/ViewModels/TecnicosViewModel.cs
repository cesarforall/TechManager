using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class TecnicosViewModel : ViewModelBase
    {
        private readonly ITecnicoService _tecnicoService;
        private readonly IServiceProvider _serviceProvider;

        ObservableCollection<Tecnico> _tecnicos;
        public RelayCommand OpenCreateTecnicoViewCommand => new RelayCommand(execute => OpenCreateTecnicoView());

        public TecnicosViewModel(ITecnicoService tecnicoService, IServiceProvider serviceProvider)
        {
            _tecnicoService = tecnicoService;
            _serviceProvider = serviceProvider;
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

        private void OpenCreateTecnicoView()
        {
            _serviceProvider.GetRequiredService<CreateTecnicoView>().Show();
        }
    }
}
