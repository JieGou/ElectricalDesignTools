using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;

namespace EDTLibrary.UndoSystem;

[AddINotifyPropertyChangedInterface]
public class UndoManager
{
    public static ObservableCollection<UndoCommand> UndoList { get; set; } = new ObservableCollection<UndoCommand>();
    public static object LockHolder { get ; set; }
    public static string LockHolderName { get => LockHolder.ToString(); }
    public static string LockProperty { get; set; }

    public static bool IsLocked { get; set; }
    public static bool IsUndoing { get; set; }

    //default is true. Only set to false when doing bulk changes and set back to true when complete
    //to always allow user changes to be added.
    public static bool CanAdd { 
        get; 
        set; } = true;

    public static void Undo()
    {
        if (UndoList.Count > 0) {
            var undoCommand = UndoList[UndoList.Count - 1];


            var itemProperties = undoCommand.Item.GetType().GetProperties();
            var prop = itemProperties.FirstOrDefault(p => p.Name == undoCommand.PropName);
            IsUndoing = true;
            prop.SetValue(undoCommand.Item, undoCommand.OldValue);
            IsUndoing = false;
            UndoList.Remove(undoCommand);
        }
    }



    public static void AddUndoCommand(UndoCommand command)
    {
        if (CanAdd == true && 
            IsUndoing == false &&
            DaManager.Importing == false &&
            DaManager.GettingRecords == false &&
            GlobalConfig.Testing == false &&
            command.NewValue != command.OldValue) {

            if (IsLocked && command.Item == LockHolder && command.PropName == LockProperty) {
                UndoList.Add(command);
                ErrorHelper.Log("UndoHelper");
                UnLock(command);
            }
            else if (IsLocked == false) {
                UndoList.Add(command);
                ErrorHelper.Log("UndoHelper");
            }
            else {
                return;
            }
        }
    }

    public static void AddUndoCommand(object item, string propName, object oldValue, object newValue)
    {
        if (CanAdd == true &&
            IsUndoing == false &&
            DaManager.Importing == false &&
            DaManager.GettingRecords == false &&
            GlobalConfig.Testing == false &&
            newValue != oldValue) {
            var command = new UndoCommand { Item = item, PropName = propName, OldValue = oldValue, NewValue = newValue};

            if (IsLocked && item == LockHolder && propName == LockProperty) {
                UndoList.Add(command);
                ErrorHelper.Log("UndoHelper");
                UnLock(command);
            }
            else if(IsLocked == false) {
                UndoList.Add(command);
                ErrorHelper.Log("UndoHelper");
            }
            else {
                return;
            }

        }
    }

    public static void Lock(object lockHolder, string lockProperty)
    {
        if (IsLocked == false &&
            IsUndoing == false &&
            DaManager.Importing == false &&
            DaManager.GettingRecords == false )
        {
            LockHolder = lockHolder;
            LockProperty = lockProperty;
            IsLocked = true;
        }
        
    }

    private static void UnLock(UndoCommand command)
    {
        if (command.Item == LockHolder && command.PropName == LockProperty) {
            IsLocked = false;
        };

    }
    public static void UnLock(object lockHolder, string lockProperty)
    {
        if (lockHolder == LockHolder && LockProperty == LockProperty) {
            IsLocked = false;
        };

    }


    public static void ClearUndoList()
    {
        UndoList.Clear();
    }
}
