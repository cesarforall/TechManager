using System.ComponentModel;
using System.Windows;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para CreateConocimientoView.xaml
    /// </summary>
    public partial class CreateConocimientoView : Window
    {
        private CreateConocimientoViewModel _viewModel;
        private EventHandler _closeHandler;

        public CreateConocimientoView(CreateConocimientoViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;

            _closeHandler = (s, e) => this.Close();
            _viewModel.RequestClose += _closeHandler;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _viewModel.RequestClose -= _closeHandler;
            base.OnClosing(e);
        }
    }
}
