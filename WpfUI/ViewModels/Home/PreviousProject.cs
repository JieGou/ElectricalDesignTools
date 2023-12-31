﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfUI.Commands;
using WpfUI.Services;

namespace WpfUI.ViewModels.Home;

[Serializable()]
public class PreviousProject
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string Project { get; }

    public PreviousProject(StartupService startupService, string project)
    {
        _startupService = startupService;
        Project = project;
        FileName = Path.GetFileName(project);
        FilePath = Path.GetFullPath(project);

    }

    private readonly StartupService _startupService;

    public void OpenProject()
    {
        _startupService.InitializeProject(Project);
        _startupService.SetSelectedProject(Project);
    }
}
