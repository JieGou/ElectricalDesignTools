using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.DataAccess;
internal class SaveController
{
    public bool IsLocked { get; set; }
    public object LockHolder { get; set; }
    public string LockProperty { get; set; }

    public void Lock(string lockProperty)
    {
        if (IsLocked == false) {
            IsLocked = true;
            LockProperty = lockProperty;
        }
    }

    //there is an additional unlock in the local function "UpdatingFedFrom_List_Check" in Dteq.FedFrom setter
    public void UnLock(string lockProperty)
    {
        if (IsLocked && lockProperty == LockProperty) {
            IsLocked = false;
        }
    }
}



