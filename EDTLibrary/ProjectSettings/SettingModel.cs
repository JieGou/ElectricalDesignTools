using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.ProjectSettings
{
    public class SettingModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }

    }
}
