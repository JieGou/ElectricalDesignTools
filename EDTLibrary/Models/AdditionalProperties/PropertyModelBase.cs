using EDTLibrary.DataAccess;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.Models.AdditionalProperties;
public abstract class PropertyModelBase
{
    public int Id { get; set; }
    public IEquipment Owner { get; set; }

    public event EventHandler PropertyUpdated;
    public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        if (DaManager.GettingRecords == true) return;

        if (PropertyUpdated != null)
        {
            PropertyUpdated(this, EventArgs.Empty);
        }
    }
}
