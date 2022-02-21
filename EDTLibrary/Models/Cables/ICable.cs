using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models.Cables
{
    public interface ICable
    {
        int Id { get; set; }
        int OwnedById { get; set; }
        string OwnedByType { get; set; }
        string Tag { get; set; }
        string Category { get; set; }
        //string Type { get; set; }
        int ConductorQty { get; set; }
        string CableSize { get; set; }

    }
}
