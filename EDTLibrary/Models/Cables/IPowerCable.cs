using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models.Cables
{
    public interface IPowerCable : ICable
    {
        double BaseAmps { get; set; }

        double Spacing { get; set; }
        double Derating { get; set; }
        double DeratedAmps { get; set; }
        double RequiredAmps { get; set; }
        double RequiredSizingAmps { get; set; }
        string AmpacityTable { get; set; }
        string InstallationDiagram { get; set; }
        IPowerCableUser Load { get; set; }

        bool SizeIsValid { get; set; }

       
    }
}
