using PropertyChanged;
using System;

namespace EDTLibrary.Models.Components;

[Serializable]
[AddINotifyPropertyChangedInterface]

public class ComponentModel : ComponentModelBase
{
    public ComponentModel()
    { }
}
