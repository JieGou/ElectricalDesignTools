using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.ProjectSettings;
using System;

namespace EDTLibrary.Autocad.Interop;
public class SingleLineDrawer
{
    public SingleLineDrawer(AutocadHelper acadHelper)
    {
        AcadHelper = acadHelper;
    }

    public AutocadHelper AcadHelper { get; }

    public void DrawMccSingleLine(MccModel mcc, int blockSpacing)
    {

        try {
            //Instert Main Block
            double[] insertionPoint = new double[3];
            insertionPoint[0] = 0;
            insertionPoint[1] = 0;
            insertionPoint[2] = 0;

            string mccBlock = "BKR";
            string blockPath = EdtSettings.AcadBlockFolder + "\\Single Line\\";
            string blockName = "MCC_MAIN_" + mccBlock + ".dwg";
            blockName = blockPath + blockName;

            double Xscale = 1;
            double Yscale = 1;
            double Zscale = 1;

            var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockName, Xscale, Yscale, Zscale, 0);
            var blockAtts = acadBlock.GetAttributes();

            foreach (AcadAttributeReference att in blockAtts) {
                switch (att.TagString) {
                    case "BUS_DETAILS":
                        att.TextString = $"{mcc.LineVoltage} V, {mcc.Size} A, 3-PH, {mcc.SCCR} kA";
                        break;
                    case "AF":
                        att.TextString = $"{mcc.PdSizeFrame} AF";
                        break;
                    case "AT":
                        att.TextString = $"{mcc.PdSizeTrip} AT";
                        break;
                    case "CABLE_TAG":
                        att.TextString = $"{mcc.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE":
                        att.TextString = $"{mcc.PowerCable.SizeTag}";
                        break;
                    case "FED_FROM":
                        att.TextString = $"{mcc.FedFrom}";
                        break;
                }
            }

            foreach (var load in mcc.AssignedLoads) {


            }
        }
        catch (Exception ex) {

            throw;
        }
    }




}
