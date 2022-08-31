using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utilities;

namespace EDTLibrary.UndoSystem;

[AddINotifyPropertyChangedInterface]
public class UndoManager
{
    public static ObservableCollection<UndoCommandDetail> UndoList { get; set; } = new ObservableCollection<UndoCommandDetail>();
    public static object LockHolder { get; set; }
    public static string LockProperty { get; set; }

    private static bool _isLocked;
    public static bool IsUndoing { get; set; }

    //default is true. Only set to false when doing bulk changes and set back to true when complete
    //to always allow user changes to be added.
    public static bool CanAdd { get; set; } = true;

    public static void UndoCommand(ListManager listManager)
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



    public static void AddUndoCommand(UndoCommandDetail command)
    {
        if (CanAdd == true && 
            IsUndoing == false &&
            DaManager.Importing == false &&
            DaManager.GettingRecords == false &&
            command.NewValue != command.OldValue) {

            if (_isLocked && command.Item == LockHolder && command.PropName == LockProperty) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UndoList.Add(command)));
                ErrorHelper.LogNoSave("UndoHelper");
                UnLock(command);
            }
            else if (_isLocked == false) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UndoList.Add(command)));
                ErrorHelper.LogNoSave("UndoHelper");
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
            newValue != oldValue) {
            var command = new UndoCommandDetail { Item = item, PropName = propName, OldValue = oldValue, NewValue = newValue };

            if (_isLocked && item == LockHolder && propName == LockProperty) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UndoList.Add(command)));
                ErrorHelper.LogNoSave("UndoHelper");
                UnLock(command);
            }
            else if(_isLocked == false) {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UndoList.Add(command)));
                ErrorHelper.LogNoSave("UndoHelper");
            }
            else {
                return;
            }

        }
    }

    public static void Lock(object lockHolder, string lockProperty)
    {
        if (_isLocked == false &&
            DaManager.Importing == false &&
            DaManager.GettingRecords == false ) {
            LockHolder = lockHolder;
            LockProperty = lockProperty;
            _isLocked = true;
        }
        
    }

    private static void UnLock(UndoCommandDetail command)
    {
        if (command.Item == LockHolder && command.PropName == LockProperty) {
            _isLocked = false;
        };
       
    }

    

    public static void ClearUndoList()
    {
        UndoList.Clear();
    }
}

public class UndoCommandDetail
{
    public object Item { get; set; }
    public string PropName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }

    public override string ToString()
    {
        var output = $"Item: {Item.ToString},Prop Name {PropName}, New Value: {NewValue.ToString}, Old Value: {OldValue}";
        return output;
    }
}
