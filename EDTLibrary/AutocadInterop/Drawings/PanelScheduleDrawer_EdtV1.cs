using AutocadLibrary;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using EdtLibrary.LibraryData.TypeModels;
using EDTLibrary.Autocad.BlockData;
using EDTLibrary.AutocadInterop.BlockData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.DistributionEquipment.DPanels;
using EDTLibrary.Models.DPanels;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Documents;

namespace EDTLibrary.Autocad.Interop;
public class PanelScheduleDrawer_EdtV1 : IPanelScheduleDrawer
{

    public PanelScheduleDrawer_EdtV1(AutocadHelper acadHelper)

    {
        AcadHelper = acadHelper;
    }

    public AutocadHelper AcadHelper { get => _acadHelper; set => _acadHelper = value; }
    private AutocadHelper _acadHelper;
    public string BlockSourceFolder { get { return EdtProjectSettings.AcadBlockFolder; } }


    public double[] _insertionPoint = new double[3];

    double startX = 5;
    double startY = 15;

    public void DrawPanelSchedule(IDteq dteq, double blockSpacing = 1.5)
    {
        //Instert Main Block
        _insertionPoint[0] = startX;
        _insertionPoint[1] = startY;
        _insertionPoint[2] = 0;

        try {

            InsertMainBlock(dteq, _insertionPoint);

            InsertBreakers(dteq, _insertionPoint);

            InsertCircuits(dteq, _insertionPoint);

        }
        catch (Exception ex) {

            throw;
        }
    }

    

    private void InsertMainBlock(IDteq dteq, double[] insertionPoint, string blockType = "Default")
    {
        

        blockType = "DP_Main";
        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        string blockName = blockType + ".dwg";
        string blockPath = sourcePath + blockName;


        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        Thread.Sleep(1000); //to prevent the connection to Autocad from faulting

        dynamic acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);

        string visbilityState = "Phase A"; //EdtProjectSettings.OverloadGraphicType;

        dynamic blockProps = acadBlock.GetDynamicBlockProperties();
        foreach (dynamic blockProp in blockProps) {

            if (blockProp.PropertyName == "Panel Phasing") {
                if (dteq.VoltageType.Poles == 3) {
                    visbilityState = "3 Phase Panel"; // Line 2
                }
                else if (dteq.VoltageType.Poles == 2) {
                    visbilityState = "Split Phase Panel"; // Line 1
                }
                else if (dteq.VoltageType.Poles == 2) {
                    visbilityState = "1 Phase Panel"; // Line 1
                }

                blockProp.Value = visbilityState;
            }
        }


        dynamic blockAtts = acadBlock.GetAttributes();

        AcadAttributeReference attr;
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

    private void InsertBreakers(IDteq dteq, double[] insertionPoint)
    {
        if (dteq.VoltageType.Poles == 3) {
            InsertBreakersFor3PhasePanel(dteq, insertionPoint);
        }
        else if (dteq.VoltageType.Poles == 2) {
            InsertBreakersForSplitPhasePanel(dteq, insertionPoint);
        }
        else if (dteq.VoltageType.Poles == 1) {
            InsertBreakersForSinglePhasePanel(dteq, insertionPoint);
        }

    }

    private void InsertBreakersForSplitPhasePanel(IDteq dteq, double[] insertionPoint)
    {
        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        //string blockName = dteq.VoltageType.Voltage == 208 && dteq.VoltageType.Phase == 3 ? "DP_3pBreakers.dwg" : "DP_1pBreakers.dwg";
        string blockName = "DP_BranchBreakers.dwg";
        string blockPath = sourcePath + blockName;
        dynamic acadBlock;

        insertionPoint[0] = startX;

        for (int i = 1; i < ((DpnModel)dteq).CircuitCount + 1; i += 2) {

            acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
            insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;

            string visbilityState = "Phase A"; //EdtProjectSettings.OverloadGraphicType;

            dynamic blockProps = acadBlock.GetDynamicBlockProperties();
            foreach (dynamic blockProp in blockProps) {

                if (blockProp.PropertyName == "Phase Connection") {
                    if (dteq.VoltageType.Poles == 2) {
                        if ((i + 1) % 4 == 0) {
                            visbilityState = "Phase C"; // Line 2
                        }
                        else if ((i + 1) % 2 == 0) {
                            visbilityState = "Phase A"; // Line 1
                        }
                    }

                    blockProp.Value = visbilityState;
                }
            }
        }
    }
    private void InsertBreakersForSinglePhasePanel(IDteq dteq, double[] insertionPoint)
    {
        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        //string blockName = dteq.VoltageType.Voltage == 208 && dteq.VoltageType.Phase == 3 ? "DP_3pBreakers.dwg" : "DP_1pBreakers.dwg";
        string blockName = "DP_BranchBreakers.dwg";
        string blockPath = sourcePath + blockName;
        dynamic acadBlock;

        insertionPoint[0] = startX;

        for (int i = 1; i < ((DpnModel)dteq).CircuitCount + 1; i += 2) {

            acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
            insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;

            string visbilityState = "Single Phase"; //EdtProjectSettings.OverloadGraphicType;

            dynamic blockProps = acadBlock.GetDynamicBlockProperties();
            foreach (dynamic blockProp in blockProps) {
                if (blockProp.PropertyName == "Phase Connection") {
                    blockProp.Value = visbilityState;
                }
            }
        }
    }
    private void InsertBreakersFor3PhasePanel(IDteq dteq, double[] insertionPoint)
    {
        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        //string blockName = dteq.VoltageType.Voltage == 208 && dteq.VoltageType.Phase == 3 ? "DP_3pBreakers.dwg" : "DP_1pBreakers.dwg";
        string blockName = "DP_BranchBreakers.dwg";
        string blockPath = sourcePath + blockName;
        dynamic acadBlock;

        insertionPoint[0] = startX;

        for (int i = 1; i < ((DpnModel)dteq).CircuitCount + 1; i += 2) {

            acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
            insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;

            string visbilityState = "Phase A"; //EdtProjectSettings.OverloadGraphicType;

            dynamic blockProps = acadBlock.GetDynamicBlockProperties();
            foreach (dynamic blockProp in blockProps) {

                if (blockProp.PropertyName == "Phase Connection") {
                    if (i == 1 || ((i - 1) % 6) == 0) {
                        visbilityState = "Phase A";
                    }
                    else if (i % 3 == 0) {
                        visbilityState = "Phase B";
                    }
                    else if ((i + 1) % 6 == 0) {
                        visbilityState = "Phase C";
                    }
                    blockProp.Value = visbilityState;
                }
            }
        }
    }

    private void InsertCircuits(IDteq dteq, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1)
    {
        insertionPoint[0] = startX;
        insertionPoint[1] = (startY - BlockVariables.DpanelMainBlockHeight);
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
        leftLineStart[0] = startX;
        leftLineEnd[0] = leftLineStart[0] + BlockVariables.DpanelCircuitRowWidth;


        //y
        leftLineStart[1] = (startY - BlockVariables.DpanelMainBlockHeight);
        leftLineEnd[1] = leftLineStart[1];


        //Right Circuits
        //x:
        rightLineStart[0] = startX + BlockVariables.DpanelBreakSectionWidth + BlockVariables.DpanelCircuitRowWidth;
        rightLineEnd[0] = startX + BlockVariables.DpanelBreakSectionWidth + BlockVariables.DpanelCircuitRowWidth * 2;


        //y:
        rightLineStart[1] = (startY - BlockVariables.DpanelMainBlockHeight);
        rightLineEnd[1] = rightLineStart[1];

        
        var panel = (DpnModel)dteq;
        dynamic acadBlock;
        dynamic blockAtts;
        dynamic line;

        //Left Circuits

        blockName = "DP_CctRowLeft" + ".dwg";
        blockPath = sourcePath + blockName;
        var cctCount = panel.LeftCircuits.Count;

        foreach (var load in panel.LeftCircuits) {
            PopulateCircuits(insertionPoint, Xscale, Yscale, Zscale, blockPath, out acadBlock, out blockAtts, load);

            line = AcadHelper.AcadDoc.ModelSpace.AddLine(leftLineStart, leftLineEnd);
            line.Layer = "E-Equipment Border Solid";

            leftLineStart[1] -= BlockVariables.DpanelCircuitRowHeight * load.VoltageType.Poles;
            leftLineEnd[1] = leftLineStart[1];
                        

        }



        //Right Circuits

        insertionPoint[0] = startX; // + BlockVariables.DpanelCircuitRowWidth + BlockVariables.DpanelBreakSectionWidth;
        insertionPoint[1] = (startY - BlockVariables.DpanelMainBlockHeight);
        insertionPoint[2] = 0;

        blockName = "DP_CctRowRight" + ".dwg";
        blockPath = sourcePath + blockName;
        cctCount = panel.RightCircuits.Count;
        foreach (var load in panel.RightCircuits) {

            PopulateCircuits(insertionPoint, Xscale, Yscale, Zscale, blockPath, out acadBlock, out blockAtts, load);

            line = AcadHelper.AcadDoc.ModelSpace.AddLine(rightLineStart, rightLineEnd);
            line.Layer = "E-Equipment Border Solid";

            rightLineStart[1] -= BlockVariables.DpanelCircuitRowHeight * load.VoltageType.Poles;
            rightLineEnd[1] = rightLineStart[1];


            

        }

        //Bottom of Panel
        line = AcadHelper.AcadDoc.ModelSpace.AddLine(leftLineStart, rightLineEnd);
        line.Layer = "E-Equipment Border Solid";


    }

    private void PopulateCircuits(double[] insertionPoint, double Xscale, double Yscale, double Zscale, string blockPath, out dynamic acadBlock, out dynamic blockAtts, IPowerConsumer load)
    {
        acadBlock = null;
        blockAtts = null;
        for (int i = 0; i < load.VoltageType.Poles; i++) {

            if (i == 0 && load.VoltageType.Poles != 3 || load.VoltageType.Poles == 3 && i == 1) {
                acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
                blockAtts = acadBlock.GetAttributes();
                foreach (dynamic att in blockAtts) {
                    switch (att.TagString) {

                        //Circuit Number
                        case "CCT":
                            att.TextString = $"{load.CircuitNumber + i * 2}";
                            break;

                        //Breaker
                        case "BRKR":
                            if (load.GetType() == typeof(LoadCircuit)) {
                                LoadCircuit loadCircuit = (LoadCircuit)load;
                                att.TextString = $"{loadCircuit.PdSizeTrip}";
                            }
                            else {
                                att.TextString = $"{load.ProtectionDevice.TripAmps}";
                            }
                            break;

                        //Watt
                        case "WATT":
                            att.TextString = $"{load.DemandKw}";
                            break;

                        //Cable
                        case "CABLE":
                            if (load.GetType() == typeof(LoadCircuit)) {
                                att.TextString = "";
                            }
                            else {
                                att.TextString = $"{load.PowerCable.QtyParallel}x #{load.PowerCable.Size}";
                            }
                            break;

                        //Description
                        case "DESC":
                            att.TextString = $"{load.Description}";
                            break;
                    }
                }
                insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;
                var y = insertionPoint[1];
            }
            else {
                //multi pole rows
                acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
                blockAtts = acadBlock.GetAttributes();
                foreach (dynamic att in blockAtts) {
                    switch (att.TagString) {

                        //Circuit Number
                        case "CCT":
                            att.TextString = $"{load.CircuitNumber + i * 2}";
                            break;

                        //Breaker
                        case "BRKR":
                            if (load.GetType() == typeof(LoadCircuit)) {
                                LoadCircuit loadCircuit = (LoadCircuit)load;
                                att.TextString = $"";
                            }
                            else {
                                att.TextString = $"";
                            }
                            break;

                        //Watt
                        case "WATT":
                            att.TextString = $"";
                            break;

                        //Cable
                        case "CABLE":
                            if (load.GetType() == typeof(LoadCircuit)) {
                                att.TextString = "";
                            }
                            else {
                                att.TextString = $"";
                            }
                            break;

                        //Description
                        case "DESC":
                            att.TextString = $"";
                            break;

                    }
                }
                insertionPoint[1] -= BlockVariables.DpanelCircuitRowHeight;
                var y2 = insertionPoint[1];
            }
        }
    }
}
