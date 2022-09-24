using EDTLibrary;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfUI.TemplateSelectors;
public class PdStarterGraphicTemplateSelector : DataTemplateSelector
{
    public DataTemplate BreakerTemplate { get; set; }
    public DataTemplate FvnrTemplate { get; set; }
    public DataTemplate FvrTemplate { get; set; }
    public DataTemplate FdsTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var selectedTemplate = BreakerTemplate;

        LoadModel load = null;
        try {
            load = (LoadModel)item;
        }
        catch (Exception) { }


        if (load == null) return selectedTemplate;

        if (load.PdType == "BKR") selectedTemplate = BreakerTemplate;
        
        if (load.PdType.Contains("FVNR"))  selectedTemplate = FvnrTemplate;

        if (load.PdType.Contains("FVR")) selectedTemplate = FvnrTemplate;
        
        if (load.PdType == "FDS")  selectedTemplate = FdsTemplate;
        

        return selectedTemplate;
    }
}
