using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para CreateConocimientoView.xaml
    /// </summary>
    public partial class CreateConocimientoView : Window
    {
        public CreateConocimientoView(ConocimientoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
