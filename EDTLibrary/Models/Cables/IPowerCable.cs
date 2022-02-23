using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models.Cables
{
    public interface IPowerCable : ICable
    {
        int CableQty { get; set; }
        double CableBaseAmps { get; set; }

        double CableSpacing { get; set; }
        double CableDerating { get; set; }
        double CableDeratedAmps { get; set; }
        double CableRequiredAmps { get; set; }
        double CableRequiredSizingAmps { get; set; }
        IPowerConsumer Load { get; set; }

    }
}
