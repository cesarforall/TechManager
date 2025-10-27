using System.Windows.Input;

namespace UI.MVVM
{
    public class RelayCommand : ICommand
    {
        // Delegados para guardar la referencia a los métodos de ejecución y verificación
        private Action<object> execute;
        private Func<object, bool> canExecute;

        // Evento que se dispara cuando cambia el estado de si se puede ejecutar el comando
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object,bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
