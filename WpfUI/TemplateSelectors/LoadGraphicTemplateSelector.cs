﻿using EDTLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.TemplateSelectors;
public class LoadGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate MotorTemplate { get; set; }
    public DataTemplate HeaterTemplate { get; set; }
    public DataTemplate PanelTemplate { get; set; }
    public DataTemplate TransformerTemplate { get; set; }
    public DataTemplate OtherTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = OtherTemplate;

        IPowerConsumer load = null;
        if (item != null) {
            try {
                if (item.GetType() == typeof(LoadModel)) {
                    load = (LoadModel)item;
                }
                else {
                    load = DteqFactory.Recast(item);
                }
            }
            catch (Exception) { }
        }

        if (load == null) return selectedTemplate;

        if (load.Type == LoadTypes.MOTOR.ToString()) {

            selectedTemplate = MotorTemplate;
        }

        else if (load.Type == LoadTypes.HEATER.ToString()) {
            selectedTemplate = HeaterTemplate;
        }
        else if (load.Type == LoadTypes.PANEL.ToString()) {
            selectedTemplate = PanelTemplate;
        }
        else if (load.Type == DteqTypes.XFR.ToString()) {
            selectedTemplate = TransformerTemplate;
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            selectedTemplate = OtherTemplate;
        }

        return selectedTemplate;
    }
}
