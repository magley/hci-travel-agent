﻿using System;
using System.Windows.Input;

namespace YouTravel.Util
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null || _canExecute == null) return false;
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter == null) return;
            _execute(parameter);
        }
    }
}
