using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfUI.ViewModels {
    class RelayCommand : ICommand {
        

        public event EventHandler? CanExecuteChanged = (sender , e) => { };

        private Action mAction;
        private Action<bool> mActionBool;
        private Action<string> mActionString;

        public RelayCommand(Action action) {
            mAction = action;
        }
        public RelayCommand(Action<bool> action)
        {
            mActionBool = action;
        }
        public RelayCommand(Action<string> action)
        {
            mActionString = action;
        }

        public bool CanExecute(object? parameter) {
            return true;
        }

        
        public void Execute(object parameter) {
            if (mAction != null) {
                mAction();
            }
            if (mActionBool != null) {
                mActionBool( (bool)parameter);
            }

            if (mActionString != null) {
                mActionString(parameter as string);
            }
        }
    }
}
