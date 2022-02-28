using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables
{
    public class CableSizeModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public bool UsedInProject { get; set; }

    }
}
