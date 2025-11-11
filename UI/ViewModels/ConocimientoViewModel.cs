using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
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

        public RelayCommand OpenCreateConocimientoViewCommand => new(execute => OpenCreateConocimientoView());

        public ConocimientoViewModel(IConocimientoService conocimientoService, IServiceProvider serviceProvider)
        {
            _conocimientoService = conocimientoService;
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
            set { _selectedGroupingOption = value; OnPropertyChanged(); }
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
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void OpenCreateConocimientoView()
        {
            var createConocimientoView = _serviceProvider.GetRequiredService<CreateConocimientoView>();
            createConocimientoView?.Show();
        }
    }
}
