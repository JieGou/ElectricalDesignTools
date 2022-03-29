using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Commands
{
    public class AsyncRelayCommand : AsyncCommandBase
    {
        private Func<Task>? _action;
        private Func<object, Task>? _actionWithParam;

        public AsyncRelayCommand(Func<Task> action, Action<Exception> onException) : base(onException)
        {
            _action = action;
        }
        public AsyncRelayCommand(Func<object, Task> action, Action<Exception> onException) :base(onException)
        {
            _actionWithParam = action;
        }
        public override async Task ExecuteAsync(object? parameter)
        {
            if (_action != null) {
                await _action();
            }
            if (_actionWithParam != null) {
                await _actionWithParam(parameter);
            }

        }
    }

}
