﻿using System;
using System.Windows.Input;
using WpfUI.Services;

namespace WpfUI.ViewModels
{
    class RelayCommand : ICommand {

        private Action? _action;
        private Func<bool>? _canExecute;
        private StartupService? _startupService;

        //public event EventHandler? CanExecuteChanged = (sender , e) => { };
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action action) {
            _action = action;
        }
      
        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public RelayCommand(Action action, StartupService startupService)
        {
            _action = action;
            _startupService = startupService;
        }

        public bool CanExecute(object? parameter) {
            if (_canExecute != null) {
                return _canExecute.Invoke();
            }
            else if (_startupService != null) {
                return _startupService.IsProjectLoaded;
            }
            else {
                return true;
            }
        }

        
        public void Execute(object? parameter) {
            if (_action != null) {
                _action();
            }
        }
    }
}