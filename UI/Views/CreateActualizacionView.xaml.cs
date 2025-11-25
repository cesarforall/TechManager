using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para CreateActualizacionView.xaml
    /// </summary>
    public partial class CreateActualizacionView : Window
    {
        public CreateActualizacionView(CreateActualizacionViewModel createActualizacionViewModel)
        {
            InitializeComponent();
            DataContext = createActualizacionViewModel;
            createActualizacionViewModel.RequestClose += (s, e) => this.Close();
        }
    }
}
