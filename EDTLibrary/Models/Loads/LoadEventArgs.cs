using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public class CalculateLoadingEventArgs : EventArgs
{
    public string PropertyName { get; set; }
}
