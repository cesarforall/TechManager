using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para UpdateTecnicoView.xaml
    /// </summary>
    public partial class UpdateTecnicoView : Window
    {
        public UpdateTecnicoView(UpdateTecnicoViewModel updateTecnicoViewModel)
        {
            InitializeComponent();
            DataContext = updateTecnicoViewModel;

            // Se suscribe al evento RequestClose del ViewModel
            updateTecnicoViewModel.RequestClose += (s, e) => this.Close();
        }
    }
}
