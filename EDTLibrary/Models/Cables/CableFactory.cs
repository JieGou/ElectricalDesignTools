using EDTLibrary.Managers;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Cables;
public class CableFactory
{
    private readonly ListManager _listManager;

    public CableFactory(ListManager listManager)
    {
        _listManager = listManager;
    }

    public static ICable CreatePowerCable(IPowerCableUser cableOwner, ListManager listManager)
    {
        ICable newCable = new CableModel(listManager);

        if (listManager.CableList.Count < 1) {
            newCable.Id = 1;
        }
        else {
            newCable.Id = listManager.CableList.Select(c => c.Id).Max() + 1;
        }
        newCable.OwnerId = cableOwner.Id;
        newCable.OwnerType = cableOwner.GetType().ToString();
        return newCable;
    }
}
