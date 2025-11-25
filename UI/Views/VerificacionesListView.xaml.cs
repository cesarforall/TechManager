using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para VerificacionesListView.xaml
    /// </summary>
    public partial class VerificacionesListView : Window
    {
        private readonly VerificacionesListViewModel _verificacionesListViewModel;

        public VerificacionesListView(VerificacionesListViewModel verificacionesListViewModel)
        {
            _verificacionesListViewModel = verificacionesListViewModel;
            InitializeComponent();
            DataContext = _verificacionesListViewModel;
            _verificacionesListViewModel.RequestClose += (s, e) => this.Close();
        }
    }
}
