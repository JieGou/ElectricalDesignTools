﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Models;

namespace WpfUI.Stores
{
    public class ProjectFileStore
    {
        private static ProjectFile? _selectedProject;
        public static ProjectFile SelectedProject
        {
            get { return _selectedProject; }
            set { _selectedProject = value; }
        }

        public static void AddToPreviouslyOpenedList()
        {

        }


        private static ProjectFile? _selectedLibrary;
        public static ProjectFile SelectedLibrary
        {
            get { return _selectedLibrary; }
            set { _selectedLibrary = value; }
        }


    }
}