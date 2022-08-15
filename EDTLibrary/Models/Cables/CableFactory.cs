using EDTLibrary.Managers;
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

    public void CreatePowerCable(IPowerConsumer powerCableUser, ListManager listManager)
    {

    }
}
