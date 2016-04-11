using System;
using System.Windows.Input;

namespace DataVisualization.Windows {
    public class DelegateCommand : ICommand {

        private readonly Action<object> _executionAction;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute) :
            this(execute, null) { }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute) {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            this._executionAction = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add {CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        #region ICommand implementation members
        public bool CanExecute(object parameter) {
            return this._canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter) {
            if (!this.CanExecute(parameter))
                throw new InvalidOperationException("The command is not valid for execution, check for canExecute before executing");

            this._executionAction(parameter);
        }
        #endregion
    }
}
