using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models
{
    public class Component
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public bool AssignCable { get; set; }

    }
}
