using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EDTLibrary.LibraryData.TypeModels
{
    public class NemaType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        [DisplayName("UL Description")]
        public string ULDescription { get; set; }
    }
}
