using EDTLibrary.DataAccess;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EDTLibrary.UndoSystem;

[AddINotifyPropertyChangedInterface]
public class UndoManager
{
    public static ObservableCollection<UndoCommandDetail> UndoList { get; set; } = new ObservableCollection<UndoCommandDetail>();

    public static bool IsUndoing { get; set; }
    public static bool CanAdd { get; set; }

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
            CanAdd = true;
        }
    }

    public static void AddUndoCommand(UndoCommandDetail command)
    {
        if (IsUndoing == false &&
            CanAdd == true &&
            DaManager.GettingRecords == false &&
            command.NewValue != command.OldValue) {
            UndoList.Add(command);
        }
    }

    public static void AddUndoCommand(object item, string propName, object oldValue, object newValue)
    {
        if (IsUndoing == false &&
            CanAdd == true &&
            DaManager.GettingRecords == false &&
            newValue != oldValue) {
            var cmd = new UndoCommandDetail { Item = item, PropName = propName, OldValue = oldValue, NewValue = newValue };
            UndoList.Add(cmd);
        }
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
