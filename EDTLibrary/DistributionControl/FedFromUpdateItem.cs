using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.DistributionControl;

/// <summary>
/// Used to send a list of loads to update their fedfrom
/// </summary>
public class UpdateFedFromItem
{
    public IPowerConsumer Caller { get; set; }
    public IDteq NewSupplier { get; set; }
    public IDteq OldSupplier { get; set; }
}

