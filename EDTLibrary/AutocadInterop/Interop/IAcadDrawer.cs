using AutocadLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.AutocadInterop.Interop;
public interface IAcadDrawer
{
    public AutocadHelper AcadHelper { get; set ; }
}
