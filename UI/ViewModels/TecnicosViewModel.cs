using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class TecnicosViewModel : ViewModelBase
    {
        private readonly ITecnicoService _tecnicoService;
        private readonly IServiceProvider _serviceProvider;

        ObservableCollection<Tecnico> _tecnicos;
        Tecnico _selectedTecnico;
        public RelayCommand OpenCreateTecnicoViewCommand => new RelayCommand(execute => OpenCreateTecnicoView());
        public RelayCommand DeleteTecnicoCommand => new RelayCommand(execute => DeleteTecnico(execute));

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

        public async Task LoadTecnicosAsync()
        {
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

        private async void DeleteTecnico(object parameter)
        {
            if (parameter is Tecnico tecnico)
            {
                if (MessageBox.Show($"¿Desea eliminar el técnico con ID: {tecnico.Id}?\n\n- Perderá todos los datos asociados al técnico.",
                    "Eliminar técnico",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var result = await _tecnicoService.delete(tecnico.Id);
                    if (result.success)
                    {
                        Tecnicos.Remove(tecnico);
                    }
                }
            }
        }
    }
}
