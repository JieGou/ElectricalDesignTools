using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.ViewModels;

namespace WpfUI.Stores
{
    public class NavigationStore: ViewModelBase    {

        public event Action? CurrentViewModelChanged;

        private ViewModelBase? _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged() {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
