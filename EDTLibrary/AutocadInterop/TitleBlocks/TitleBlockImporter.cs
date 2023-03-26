using AutocadLibrary;
using EDTLibrary.Autocad.Interop;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Services;
using EDTLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.AutocadInterop.TitleBlocks;
public class TitleBlockImporter
{
    public TitleBlockImporter(AutocadHelper acadHelper)

    {
        AcadHelper = acadHelper;
    }

    public AutocadHelper AcadHelper { get => _acadHelper; set => _acadHelper = value; }
    private AutocadHelper _acadHelper;

    public string BlockSourceFolder { get { return EdtProjectSettings.AcadBlockFolder; } }

    public List<CadAttribute> ImportTitleBlock(string blockPath)
    {

        var attributeList = new List<CadAttribute>();

        double[] insertionPoint = new double[3];
        insertionPoint[0] = 0;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;

        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        var acadBlock = _acadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        var blockAtts = (dynamic)acadBlock.GetAttributes();
        foreach (var att in blockAtts) {
            attributeList.Add(
                new CadAttribute { TagString = att.TagString, TextString = att.TextString });
        }

        return attributeList;
        
    }
}
