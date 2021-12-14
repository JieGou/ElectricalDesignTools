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
    class SelectProjectCommand : CommandBase
    {
        private readonly ProjectSettingsViewModel? _viewModel;
        private readonly NavigationStore? _navigationsStore;

        public SelectProjectCommand(ProjectSettingsViewModel? viewModel, NavigationStore? navigationsStore)
        {
            _viewModel = viewModel;
            _navigationsStore = navigationsStore;
        }

        public override void Execute(object? parameter)
        {
            MessageBox.Show($"Selected Project: {_viewModel.CurrentProject}");
            _navigationsStore.CurrentViewModel = new ProjectSettingsViewModel(_navigationsStore);
        }
    }
}
