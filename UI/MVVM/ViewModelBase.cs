using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UI.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Este método se llama para notificar a la vista que una propiedad ha cambiado
        // El atributo CallerMemberName hace que el compilador llene automáticamente el nombre de la propiedad que llamó al método
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
