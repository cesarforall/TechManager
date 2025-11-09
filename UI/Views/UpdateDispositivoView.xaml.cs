using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para UpdateDispositivoView.xaml
    /// </summary>
    public partial class UpdateDispositivoView : Window
    {
        public UpdateDispositivoView(UpdateDispositivoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += (s, e) => this.Close();
        }
    }
}
