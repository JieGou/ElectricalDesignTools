﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary;
public class Undo
{
    public static List<CommandDetail> UndoList { get; set; } = new List<CommandDetail>();

    public static bool Undoing { get; set; }
    public static void UndoCommand(ListManager listManager)
    {
        if (UndoList.Count>0) {
            var undoCmd = UndoList[UndoList.Count - 1];

            
            var itemProperties = undoCmd.Item.GetType().GetProperties();
            var prop = itemProperties.FirstOrDefault(p => p.Name == undoCmd.PropName);
            Undoing = true;
                prop.SetValue(undoCmd.Item, undoCmd.OldValue);
            Undoing = false;
            UndoList.Remove(undoCmd);
        }
    }
}

public class CommandDetail
{
    public object Item { get; set; }
    public string PropName { get; set; }
    public object OldValue { get; set; }
    public object NewValue { get; set; }
}