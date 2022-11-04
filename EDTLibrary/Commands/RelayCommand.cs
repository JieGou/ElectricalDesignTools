using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Windows.Input;

namespace EdtLibrary.Commands
{
    [Serializable]
    public class RelayCommand : ICommand
    {

        private Action? _action;
        private Action<object> _actionWithParam;
        private Func<bool>? _canExecute;

        //public event EventHandler? CanExecuteChanged = (sender , e) => { };
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } //This is called during InitializeProject to check fire CanExecuteChanged;
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action action)
        {
            _action = action;
        }

        public RelayCommand(Action<object> action)
        {
            _actionWithParam = action;
        }

        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }
       
        public bool CanExecute(object? parameter)
        {
            return true; 
            
            //if (_canExecute != null) {
            //    return _canExecute.Invoke();
            //}
            //else {
            //    return true;
            //}
        }


        public void Execute(object? parameter)
        {
            if (_action != null) {
                _action();
            }
            if (_actionWithParam != null) {
                _actionWithParam(parameter);
            }
        }
    }
}
