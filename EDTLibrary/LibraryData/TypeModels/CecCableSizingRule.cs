using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData.TypeTables
{
    public class CecCableSizingRule
    {
        public int Id { get; set; }
        public string CableType { get; set; }
        public string InstallationType { get; set; }
        public double Spacing { get; set; }
        public bool Outdoor { get; set; }
        public string AmpacityTable { get; set; }

    }
}
