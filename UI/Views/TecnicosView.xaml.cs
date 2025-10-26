using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Lógica de interacción para TecnicosView.xaml
    /// </summary>
    public partial class TecnicosView : UserControl
    {
        public TecnicosView(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            DataContext = serviceProvider.GetRequiredService<TecnicosViewModel>();
        }
    }
}
