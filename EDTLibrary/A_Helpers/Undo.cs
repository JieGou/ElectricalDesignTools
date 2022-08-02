using EDTLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary;
public class Undo
{
    public static List<UndoCommandDetail> UndoList { get; set; } = new List<UndoCommandDetail>();

    public static bool Undoing { get; set; }
    public static void UndoCommand(ListManager listManager)
    {
        if (UndoList.Count>0) {
            var undoCommand = UndoList[UndoList.Count - 1];

            
            var itemProperties = undoCommand.Item.GetType().GetProperties();
            var prop = itemProperties.FirstOrDefault(p => p.Name == undoCommand.PropName);
            Undoing = true;
                prop.SetValue(undoCommand.Item, undoCommand.OldValue);
            Undoing = false;
            UndoList.Remove(undoCommand);
        }
    }

    public static void AddUndoCommand(UndoCommandDetail command)
    {
        if (Undo.Undoing == false && DaManager.GettingRecords == false) {
            UndoList.Add(command);
        }
    }

    public static void AddUndoCommand(object item, string propName, object oldValue, object newValue)
    {
        if (Undo.Undoing == false && DaManager.GettingRecords == false) {
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
}
