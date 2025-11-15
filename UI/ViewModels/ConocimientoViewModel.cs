using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using UI.MVVM;
using UI.Views;

namespace UI.ViewModels
{
    public class ConocimientoViewModel : ViewModelBase
    {
        private readonly IConocimientoService _conocimientoService;
        private readonly IServiceProvider _serviceProvider;
        private ObservableCollection<Conocimiento> _conocimientos;
        private List<string> _groupingOptions;
        private string _selectedGroupingOption = "Todos";
        private ICollectionView _groupedConocimientos;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        public RelayCommand OpenCreateConocimientoViewCommand => new(execute => OpenCreateConocimientoView());
        public RelayCommand DeleteConocimientoCommand => new(execute => DeleteConocimiento(execute));
        public RelayCommand ToggleSortCommand => new(execute => ToggleSort());

        public ConocimientoViewModel(IConocimientoService conocimientoService, IServiceProvider serviceProvider)
        {
            _conocimientoService = conocimientoService;
            _conocimientoService.ConocimientoCreated += OnConocimientoCreated;
            _serviceProvider = serviceProvider;
            _conocimientos = new();
            LoadConocimientosAsync();
            _groupingOptions = new List<string> { "Todos", "Técnicos", "Dispositivos" };
        }

        public ObservableCollection<Conocimiento> Conocimientos
        {
            get { return _conocimientos; }
            set { _conocimientos = value; OnPropertyChanged(); }
        }

        public List<string> GroupingOptions
        {
            get { return _groupingOptions; }
            set { _groupingOptions = value; OnPropertyChanged(); }
        }

        public string SelectedGroupingOption
        {
            get { return _selectedGroupingOption; }
            set
            {
                if (_selectedGroupingOption != value)
                {
                    _selectedGroupingOption = value;
                    OnPropertyChanged();

                    _groupedConocimientos = null;
                    OnPropertyChanged(nameof(GroupedConocimientos));
                }
            }
        }

        public ICollectionView GroupedConocimientos
        {
            get
            {
                if (_groupedConocimientos == null)
                {
                    _groupedConocimientos = CollectionViewSource.GetDefaultView(Conocimientos);
                    _groupedConocimientos.GroupDescriptions.Clear();

                    string? description = SelectedGroupingOption switch
                    {
                        "Técnicos" => "Tecnico.FullName",
                        "Dispositivos" => "Dispositivo.FullName",
                        _ => null
                    };

                    _groupedConocimientos.GroupDescriptions.Add(
                        new PropertyGroupDescription(description));
                }
                return _groupedConocimientos;
            }
        }

        public async void LoadConocimientosAsync()
        {
            try
            {
                var (success, message, conocimientos) = await _conocimientoService.GetAll();

                if (success)
                {
                    foreach (var conocimiento in conocimientos)
                    {
                        Conocimientos.Add(new Conocimiento
                        {
                            Id = conocimiento.Id,
                            TecnicoId = conocimiento.TecnicoId,
                            Tecnico = conocimiento.Tecnico,
                            DispositivoId = conocimiento.DispositivoId,
                            Dispositivo = conocimiento.Dispositivo
                        });                        
                    }

                    Conocimientos.OrderDescending();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void DeleteConocimiento(object parameter)
        {
            if (parameter is Conocimiento conocimiento && conocimiento.Id is int id)
            {
                if (MessageBox.Show($"¿Desea desasociar el conocimiento con ID: {id}?",
                    "Eliminar conocimiento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var result = await _conocimientoService.Delete(id);
                    if (result.success)
                    {
                        Conocimientos.Remove(conocimiento);
                    }
                }
            }
        }

        public void OpenCreateConocimientoView()
        {
            var createConocimientoView = _serviceProvider.GetRequiredService<CreateConocimientoView>();
            createConocimientoView?.Show();
        }

        private async void OnConocimientoCreated(object? sender, int conocimientoId)
        {
            var result = await _conocimientoService.GetById(conocimientoId);
            if (result.success)
            {
                _conocimientos.Add(result.conocimiento);
            }
        }
        
        private void ToggleSort()
        {
            string description = "Tecnicos.FullName";

            switch (SelectedGroupingOption)
            {
                case "Técnicos":
                    description = "Tecnico.FullName";
                    break;
                case "Dispositivos":
                    description = "Dispositivo.FullName";
                    break;
                default:
                    description = "Tecnico.FullName";
                    break;
            }

            _sortDirection = _sortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            GroupedConocimientos.SortDescriptions.Clear();
            GroupedConocimientos.SortDescriptions.Add(
                new SortDescription(description, _sortDirection));
            GroupedConocimientos.Refresh();
        }
    }
}
