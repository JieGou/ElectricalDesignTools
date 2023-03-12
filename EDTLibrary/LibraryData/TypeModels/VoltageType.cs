using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EdtLibrary.LibraryData.TypeModels
{
    public class VoltageType : UserEditableTypeBase
    {
        public double Voltage
        {
            get;
            set;
        }
        public double Phase { get; set; }
        public double Frequency { get; set; }

        [DisplayName("Display Text")]
        [Description("Display Text")]
        public string VoltageString
        {
            get;
            set;
        }
        public int Poles { get; set; }




    }
}
