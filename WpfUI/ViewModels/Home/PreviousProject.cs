using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;

namespace WpfUI.ViewModels.Home;
public class PreviousProject
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FullPath { get; }

    public PreviousProject(StartupService startupService, string project)
    {
        _startupService = startupService;
        FullPath = project;
        FileName = Path.GetFileName(project);
        FilePath = Path.GetFullPath(project);

        OpenProjectCommand = new RelayCommand(OpenProject);

    }
    public ICommand OpenProjectCommand;

    private readonly StartupService _startupService;

    public void OpenProject()
    {
        _startupService.InitializeProject(FullPath);
        _startupService.SetSelectedProject(FullPath);
    }
}
