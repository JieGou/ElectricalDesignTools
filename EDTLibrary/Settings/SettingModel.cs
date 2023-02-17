using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace EDTLibrary.ProjectSettings
{
    [AddINotifyPropertyChangedInterface]
    public class SettingModel: INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Value { get; set; }
        public DataTable TableValue { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
