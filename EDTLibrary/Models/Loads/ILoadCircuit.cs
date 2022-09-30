using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Loads;
public interface ILoadCircuit : ILoad
{
   
    event EventHandler SpaceConverted;

    Task OnSpaceConverted();

}