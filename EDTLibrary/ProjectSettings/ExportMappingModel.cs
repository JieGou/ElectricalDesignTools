using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace EDTLibrary.ProjectSettings
{
    [AddINotifyPropertyChangedInterface]
    public class ExportMappingModel
    {
        public int Id { get; set; }
        public string  Type { get; set; }
        public string PropertyName { get; set; }
        public string Name { get; set; }
        public bool IncludeInReport { get; set; }
     
    }
}
