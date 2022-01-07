﻿using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.Stores
{
    public class ListStore
    {
        public ObservableCollection<DteqModel> DteqList { get; set; }
        public ObservableCollection<LoadModel> LoadList { get; set; }
        public ObservableCollection<CableModel> CableList { get; set; }

    }
}