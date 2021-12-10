using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfUI.ViewModels {
    class RelayCommand : ICommand {
        #region Public Events
        /// <summary>
        /// The event that's fired when the <see cref="CanExecute(object?)"/> value has changed
        /// </summary>
        public event EventHandler? CanExecuteChanged = (sender , e) => { };
        #endregion

        #region Private Members
        private Action mAction;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action action) {
            mAction = action;
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// A Relay Command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter) {
            return true;
        }

        /// <summary>
        /// Executes the command Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter) {
            mAction();
        }
        #endregion
    }
}
