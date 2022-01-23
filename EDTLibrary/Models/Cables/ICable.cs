using System;
using System.Collections.Generic;
using System.Text;

namespace EDTLibrary.Models
{
    public interface ICable
    {
        int Id { get; set; }
        string Tag { get; set; }
        string Category { get; set; }
        string Type { get; set; }
        int ConductorQty { get; set; }
        string CableSize { get; set; }
    }
}
