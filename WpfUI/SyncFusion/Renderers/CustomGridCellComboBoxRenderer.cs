﻿using EDTLibrary;
using EDTLibrary.Models.Loads;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfUI.SyncFusion.Renderers;

public class CustomGridCellComboBoxRenderer : GridCellComboBoxRenderer
{
    public override void OnInitializeEditElement(DataColumnBase dataColumn, ComboBox uiElement, object dataContext)
    {
        base.OnInitializeEditElement(dataColumn, uiElement, dataContext);
        //uiElement.Template = App.Current.Resources["ComboBoxBase"] as ControlTemplate;
    }
    protected override void OnEditElementLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        base.OnEditElementLoaded(sender, e);
        var combobox = sender as ComboBox;
        combobox.IsDropDownOpen = true;
    }
    

}