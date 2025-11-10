using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para ConocimientosView.xaml
    /// </summary>
    public partial class ConocimientosView : UserControl
    {
        public ConocimientosView(ConocimientoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
