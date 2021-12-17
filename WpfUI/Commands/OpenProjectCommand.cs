using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfUI.ViewModels;
using WpfUI.Stores;
using WpfUI.Services;
using WpfUI.Models;

namespace WpfUI.Commands
{
    class OpenProjectCommand : CommandBase
    {
        private readonly StartupViewModel? _viewModel;
        private readonly ProjectFileStore _projectFileStore;
        private readonly NavigationService<ProjectSettingsViewModel> _navigationService;

        public OpenProjectCommand(StartupViewModel? viewModel, ProjectFileStore projectFileStore, NavigationService<ProjectSettingsViewModel> navigationService)
        {
            _viewModel = viewModel;
            _projectFileStore = projectFileStore;
            _navigationService = navigationService;
        }

        //public OpenProjectCommand(StartupViewModel? viewModel, NavigationService<ProjectSettingsViewModel> navigationService)
        //{
        //    _viewModel = viewModel;
        //    _navigationService = navigationService;
        //}


        public override void Execute(object? parameter)
        {
            ProjectFile projectFile = new ProjectFile() {
                Name = _viewModel.ProjectName,
                Path = _viewModel.ProjectPath
            };
            MessageBox.Show($"Opening Project: {_viewModel.ProjectName}");

            _projectFileStore.SelectedProject = projectFile;
            //Navigate to Poject Settings page
            _navigationService.Navigate();
        }
    }
}
