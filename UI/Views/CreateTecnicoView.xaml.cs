using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para CreateTecnicoView.xaml
    /// </summary>
    public partial class CreateTecnicoView : Window
    {
        public CreateTecnicoView(CreateTecnicoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Se suscribe al evento RequestClose del ViewModel
            viewModel.RequestClose += (s, e) => this.Close();
        }
    }
}

