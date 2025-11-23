using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para ActualizacionesView.xaml
    /// </summary>
    public partial class ActualizacionesView : UserControl
    {
        public ActualizacionesView(ActualizacionesViewModel actualizacionesViewModel)
        {
            InitializeComponent();
            DataContext = actualizacionesViewModel;
        }
    }
}
