using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models.Cables
{
    public interface IPowerCable : ICable
    {
        int QtyParallel { get; set; }
        double BaseAmps { get; set; }

        double Spacing { get; set; }
        double Derating { get; set; }
        double DeratedAmps { get; set; }
        double RequiredAmps { get; set; }
        double RequiredSizingAmps { get; set; }
        string InstallationType { get; set; }
        bool Indoor { get; set; }
        IPowerConsumer Load { get; set; }

    }
}
