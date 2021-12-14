using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.ViewModels;
using WpfUI.Stores;

namespace WpfUI.Commands
{
    class OpenProjectCommand : CommandBase
    {
        private readonly StartupViewModel? _viewModel;
        private readonly NavigationStore? _navigationsStore;

        public OpenProjectCommand(StartupViewModel? viewModel, NavigationStore? navigationsStore)
        {
            _viewModel = viewModel;
            _navigationsStore = navigationsStore;
        }

        public override void Execute(object? parameter)
        {
            MessageBox.Show($"Opening Project: {_viewModel.SelectedProject}");
            
            //Navigate to Poject Settings page
            _navigationsStore.CurrentViewModel = new ProjectSettingsViewModel(_navigationsStore);
        }
    }
}
