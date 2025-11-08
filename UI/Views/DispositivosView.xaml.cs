using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para DispositivosView.xaml
    /// </summary>
    public partial class DispositivosView : UserControl
    {
        DispositivosViewModel _dispositivosViewModel;
        public DispositivosView(DispositivosViewModel dispositivosViewModel)
        {
            _dispositivosViewModel = dispositivosViewModel;
            InitializeComponent();
            DataContext = _dispositivosViewModel;
        }
    }
}
