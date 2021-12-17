using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Stores;
using WpfUI.ViewModels;

namespace WpfUI.Services
{
    public class ParameterNavigationService<TParameter, TViewModel>
        where TViewModel : ViewModelBase
    {
        private readonly NavigationStore? _navigationsStore;
        private readonly Func<TParameter, TViewModel> _createViewModel;

        public ParameterNavigationService(NavigationStore? navigationsStore, Func<TParameter, TViewModel> createViewModel)
        {
            _navigationsStore = navigationsStore;
            _createViewModel = createViewModel;
        }

        public void Navigate(TParameter parameter)
        {
            _navigationsStore.CurrentViewModel = _createViewModel(parameter);
        }

    }
}
