using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData.Cables
{
    /// <summary>
    /// CecCableAmpacityTable from Library Db
    /// </summary>
    public class CecCableAmpacityModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Size { get; set; }
        public double Amps60 { get; set; }
        public double Amps75 { get; set; }
        public double Amps90 { get; set; }
        public string AmpacityTable { get; set; }

    }
}
