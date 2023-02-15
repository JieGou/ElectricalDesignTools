using AutocadLibrary;
using Autodesk.AutoCAD.Interop.Common;
using EDTLibrary.Autocad.BlockData;
using EDTLibrary.AutocadInterop.BlockData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;
using System.Security.Cryptography;
using System.Windows.Documents;

namespace EDTLibrary.Autocad.Interop;
public class PanelScheduleDrawer
{

    public PanelScheduleDrawer(AutocadHelper acadHelper, string blockSourceFolder)

    {
        _acadHelper = acadHelper;
        BlockSourceFolder = blockSourceFolder;
    }

    private AutocadHelper _acadHelper;
    public string BlockSourceFolder { get; }

    public double[] _insertionPoint = new double[3];
    int _firstLoadSpacing = 1;

    public void DrawPanelSchedule(IDteq dteq, double blockSpacing = 1.5)
    {

        try {
 
            InsertMainBlock(dteq, _insertionPoint);

            InsertCircuits(dteq, _insertionPoint);


           
        }
        catch (Exception ex) {

            throw;
        }
    }

    private void InsertMainBlock(IDteq dteq, double[] insertionPoint, string blockType = "Default")
    {
        //Instert Main Block
        insertionPoint[0] = 0;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;

        blockType = dteq.VoltageType.Phase == 3 ? BlockNames.Dp3PhMain : BlockNames.Dp1PhMain;
        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        string blockName = blockType + ".dwg";
        string blockPath = sourcePath + blockName; 
     

        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        
        dynamic acadBlock = _acadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        dynamic blockAtts = acadBlock.GetAttributes();

        //AcadAttributeReference
        foreach (dynamic att in blockAtts) {
            switch (att.TagString) {
                case "PANEL_TAG":
                    att.TextString = $"{dteq.Tag}";
                    break;
                case "PANEL_DESCRIPTION":
                    att.TextString = $"{dteq.Description}";
                    break;
                case "PANEL_AREA":
                    att.TextString = $"{dteq.Area.Tag}";
                    break;
                case "PANEL_AMPS":
                    att.TextString = $"{dteq.Size} A";
                    break;
                case "PANEL_VOLTAGE":
                    att.TextString = $"{dteq.LineVoltage} V, {dteq.Size} A, 3-PH";
                    break;
                case "PANEL_SCCR":
                    att.TextString = $"{dteq.SCCR} kA";
                    break;
                case "AF":
                    att.TextString = $"{dteq.ProtectionDevice.FrameAmps} AF";
                    break;
                case "AT":
                    att.TextString = $"{dteq.ProtectionDevice.TripAmps} AT";
                    break;
                case "CABLE_TAG":
                    att.TextString = $"{dteq.PowerCable.Tag}";
                    break;
                case "CABLE_SIZE":
                    att.TextString = $"{dteq.PowerCable.SizeTag}";
                    break;
                case "FED_FROM":
                    att.TextString = $"{dteq.FedFrom}";
                    break;
            }
        }

        


    }
    private void InsertCircuits(IDteq dteq, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1)
    {
        var panel = (IDpn)dteq;
        insertionPoint[0] = 0;
        insertionPoint[1] = BlockVariables.DpanelMainBlockHeight *-1;
        insertionPoint[2] = 0;

        string blockType = BlockNames.DpCircuit;
        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        string blockName = blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        double[] leftLineStart = new double[3];
        double[] leftLineEnd = new double[3];
        double[] rightLineStart = new double[3];
        double[] rightLineEnd = new double[3];


        //  FIRST INSERTION POINTS
        //  Left Circuits
        //  x
        leftLineEnd[0] = BlockVariables.DpanelCircuitRowWidth;


        //y
        leftLineStart[1] = BlockVariables.DpanelMainBlockHeight * -1;
        leftLineEnd[1] = leftLineStart[1];


        //Right Circuits
        //x:
        rightLineStart[0] = BlockVariables.DpanelBreakSectionWidth + BlockVariables.DpanelCircuitRowWidth;
        rightLineEnd[0] = BlockVariables.DpanelBreakSectionWidth + BlockVariables.DpanelCircuitRowWidth * 2;


        //y:
        rightLineStart[1] = BlockVariables.DpanelMainBlockHeight * -1;
        rightLineEnd[1] = rightLineStart[1];



        for (int i = 0; i < panel.CircuitCount/2; i++) {

            dynamic acadBlock = _acadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);

            dynamic blockAtts = acadBlock.GetAttributes();

            if (panel.LeftCircuits[i].PanelSide==PnlSide.Left.ToString()) {
                foreach (dynamic att in blockAtts) {
                    switch (att.TagString) {

                        //Circuit Number
                        case DistributionPanelAttributeNames.Dpanel_CctL:
                            att.TextString = $"{panel.LeftCircuits[i].CircuitNumber}";
                            break;

                        //Breaker
                        case DistributionPanelAttributeNames.Dpanel_BreakerL:
                            if (panel.LeftCircuits[i].GetType() == typeof(LoadCircuit)) {
                                LoadCircuit loadCircuit = (LoadCircuit)panel.LeftCircuits[i];
                                att.TextString = $"{loadCircuit.PdSizeTrip}"; 
                            }
                            else   {
                                att.TextString = $"{panel.LeftCircuits[i].ProtectionDevice.TripAmps}";
                            }
                            break;

                        //Watt
                        case DistributionPanelAttributeNames.Dpanel_WattL:
                            att.TextString = $"{panel.LeftCircuits[i].DemandKw}";
                            break;

                        //Cable
                        case DistributionPanelAttributeNames.Dpanel_CableL:
                            if (panel.LeftCircuits[i].GetType() == typeof(LoadCircuit)) {
                                att.TextString = "";
                            }
                            else {
                                att.TextString = $"{panel.LeftCircuits[i].PowerCable.QtyParallel}x #{panel.LeftCircuits[i].PowerCable.Size}";
                            }
                            break;

                        //Description
                        case DistributionPanelAttributeNames.Dpanel_DescriptionL:
                            att.TextString = $"{panel.LeftCircuits[i].Description}";
                            break;

                    }
                } 
            }
          
            if (panel.RightCircuits[i].PanelSide == PnlSide.Right.ToString()) {
                foreach (dynamic att in blockAtts) {
                    switch (att.TagString) {

                        //Circuit Number
                        case DistributionPanelAttributeNames.Dpanel_CctR:
                            att.TextString = $"{panel.RightCircuits[i].CircuitNumber}";
                            break;

                        //Breaker
                        case DistributionPanelAttributeNames.Dpanel_BreakerR:
                            if (panel.RightCircuits[i].GetType() == typeof( LoadCircuit)) {
                                LoadCircuit loadCircuit = (LoadCircuit)panel.RightCircuits[i];
                                att.TextString = $"{loadCircuit.PdSizeTrip}";
                            }
                            else {
                                
                                att.TextString = $"{panel.RightCircuits[i].ProtectionDevice.TripAmps}";
                            }
                            break;

                        //Watt
                        case DistributionPanelAttributeNames.Dpanel_WattR:
                            att.TextString = $"{panel.RightCircuits[i].DemandKw}";
                            break;

                        //Cable
                        case DistributionPanelAttributeNames.Dpanel_CableR:
                            if (panel.RightCircuits[i].GetType() == typeof(LoadCircuit)) {
                                att.TextString = "";
                            }
                            else {
                                att.TextString = $"{panel.RightCircuits[i].PowerCable.QtyParallel}x #{panel.RightCircuits[i].PowerCable.Size}";
                            }
                            break;

                        //Description
                        case DistributionPanelAttributeNames.Dpanel_DescriptionR:
                            att.TextString = $"{panel.RightCircuits[i].Description}";
                            break;

                    }
                }
                
            }

            //increment insertion points
            insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;

            dynamic line = _acadHelper.AcadDoc.ModelSpace.AddLine(leftLineStart, leftLineEnd);
            line.Layer = "E-ANNO-TABL";

            leftLineStart[1] -= BlockVariables.DpanelCircuitRowHeight;
            leftLineEnd[1] = leftLineStart[1];

            line = _acadHelper.AcadDoc.ModelSpace.AddLine(rightLineStart, rightLineEnd);
            line.Layer = "E-ANNO-TABL";

            rightLineStart[1] -= BlockVariables.DpanelCircuitRowHeight;
            rightLineEnd[1] = rightLineStart[1];
        }

    }


}
