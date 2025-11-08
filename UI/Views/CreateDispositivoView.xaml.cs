using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para CreateDispositivoView.xaml
    /// </summary>
    public partial class CreateDispositivoView : Window
    {
        public CreateDispositivoView(CreateDispositivoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += (s, e) => this.Close();
        }
    }
}
