using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[Serializable]
[AddINotifyPropertyChangedInterface]

public class ComponentModel : ComponentModelBase
{

    public ComponentModel()
    {
        //Category = Categories.Component.ToString();
    }

 
}
