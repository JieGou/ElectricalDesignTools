using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Autocad.BlockData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;
using System.Windows.Documents;

namespace EDTLibrary.Autocad.Interop;
public class PanelScheduleDrawer
{

    public PanelScheduleDrawer(AutocadHelper acadService, string blockSourceFolder)

    {
        _acad = acadService;
        BlockSourceFolder = blockSourceFolder;
    }

    private AutocadHelper _acad;
    public string BlockSourceFolder { get; }

    public double[] _insertionPoint = new double[3];
    int firstLoadSpacing = 1;

    public void DrawPanelSchedule(IDteq dteq, double blockSpacing = 1.5)
    {

        try {
 
            InsertMainBlock(dteq, _insertionPoint);

            InsertCircuits(dteq, _insertionPoint);


            InsertMccBus(dteq, _insertionPoint, blockSpacing);
            InsertMccBorder(dteq, _insertionPoint, blockSpacing);
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

        
        var acadBlock = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        var blockAtts = (dynamic)acadBlock.GetAttributes();

        
        foreach (AcadAttributeReference att in blockAtts) {
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
                    att.TextString = $"{dteq.Size} AF";
                    break;
                case "PANEL_VOLTAGE":
                    att.TextString = $"{dteq.LineVoltage} V, {dteq.Size} A, 3-PH, {dteq.SCCR} kA";
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

        insertionPoint[0] = 0;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;

        string blockType = BlockNames.DpCircuit;
        string sourcePath = BlockSourceFolder + FolderNames.DistributionPanelsFolder;
        string blockName = blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        double[] linePoint1L = new double[3];
        double[] linePoint2L = new double[3];
        double[] linePoint1R = new double[3];
        double[] linePoint2R = new double[3];


        //  FIRST INSERTION POINTS
        //  Left Circuits
        //  x
        linePoint2L[1] = BlockVariables.DpanelCircuitRowWidth;


        //y
        linePoint1L[2] = BlockVariables.DpanelMainBlockHeight * -1;
        linePoint2L[2] = linePoint1L[2];


        //Right Circuits
        //x:
        linePoint1R[1] = BlockVariables.DpanelCircuitRowWidth + BlockVariables.DpanelBreakSectionWidth;
        linePoint2R[1] = 2 * BlockVariables.DpanelCircuitRowWidth + BlockVariables.DpanelBreakSectionWidth;


        //y:
        linePoint1R[2] = BlockVariables.DpanelMainBlockHeight * -1;
        linePoint2R[2] = linePoint1R[2];

        
        var acadBlock = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        
        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (AcadAttributeReference att in blockAtts) {
            switch (att.TagString) {

                //Starter
                case "MCP_Size":
                    att.TextString = $"{dteq.ProtectionDevice.TripAmps} A";
                    break;
               

                //Breaker
                case "AT":
                    att.TextString = $"{dteq.ProtectionDevice.TripAmps}";
                    break;
                case "AF":
                    att.TextString = $"{dteq.ProtectionDevice.FrameAmps}";
                    break;

                //Drive
                case "DRIVE_TYPE":
                    if (dteq.Drive != null)
                        att.TextString = $"{dteq.Drive.Type}";
                    break;
               

            }
        }


    }
    private void InsertLoad(LoadModel load, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1, bool isDriveInternal = false)
    {

        //default Load
        string blockType = "Load";
        var tag = load.Tag;

        //select load type
        if (load.Type == LoadTypes.MOTOR.ToString()) {
            blockType = "Motor";
        }
        else if (load.Type == LoadTypes.HEATER.ToString()) {
            blockType = "Load";
        }
        else if (load.Type == LoadTypes.WELDING.ToString()) {
            blockType = "WLD";
        }
        else if (load.Type == LoadTypes.OTHER.ToString()) {
            blockType = "Load";
        }

        if (load.DriveBool == true && isDriveInternal == false) {
            blockType = "Drive";
        }

        if (load.DisconnectBool == true) {
            blockType += "_DCN";
        }


        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "SL_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        
        //instert block
        var acadBlock = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);

        var blockAtts = (dynamic)acadBlock.GetAttributes();

        //update attributes
        foreach (AcadAttributeReference att in blockAtts) {
            switch (att.TagString) {

                //Cable 1
                case "CABLE_TAG":
                    att.TextString = $"{load.PowerCable.Tag} A";
                    break;
                case "CABLE_SIZE":
                    att.TextString = $"{load.PowerCable.SizeTag} A";
                    break;

                //Breaker
                case "LOAD_SIZE":
                    att.TextString = load.Type == LoadTypes.MOTOR.ToString()? $"{load.Size}" : $"{load.Size} {load.Unit}";
                    break;
                case "LOAD_TAG":
                    att.TextString = $"{load.Tag}";
                    break;
                case "LOAD_DESCRIPTION":
                    att.TextString = $"{load.Description}";
                    break;

                //Drive
                case "DRIVE_TAG":
                    if (load.Drive != null)
                        att.TextString = $"{load.Drive.Tag}";
                    break;
                case "DRIVE_TYPE":
                    if (load.Drive != null)
                        att.TextString = $"{load.Drive.Type}";
                    break;
                

                //Disconnect
                case "DCN_TAG":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.Tag}";
                    break;
                case "DCN_SIZE":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.FrameAmps}";
                    break;
            }

            //Disconnect and Drive
            if (load.DisconnectBool == true && load.DriveBool == true && isDriveInternal == false) {
                switch (att.TagString) {
                    case "CABLE_TAG1":
                        att.TextString = $"{load.Drive.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE1":
                        att.TextString = $"{load.Drive.PowerCable.SizeTag}";
                        break;
                    case "CABLE_TAG2":
                        att.TextString = $"{load.Disconnect.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE2":
                        att.TextString = $"{load.Disconnect.PowerCable.SizeTag}";
                        break;
                    case "CABLE_TAG3":
                        att.TextString = $"{load.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE3":
                        att.TextString = $"{load.PowerCable.SizeTag}";
                        break;
                }
            }

            //Disconnect only
            else if (load.DisconnectBool == true && load.DriveBool == false) {
                switch (att.TagString) {
                    case "CABLE_TAG1":
                        att.TextString = $"{load.Disconnect.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE1":
                        att.TextString = $"{load.Disconnect.PowerCable.SizeTag}";
                        break;
                    case "CABLE_TAG2":
                        att.TextString = $"{load.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE2":
                        att.TextString = $"{load.PowerCable.SizeTag}";
                        break;
                }
            }
            //Drive only
            else if (load.DisconnectBool == false && load.DriveBool == true && isDriveInternal==false) {
                switch (att.TagString) {
                    case "CABLE_TAG1":
                        att.TextString = $"{load.Drive.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE1":
                        att.TextString = $"{load.Drive.PowerCable.SizeTag}";
                        break;
                    case "CABLE_TAG2":
                        att.TextString = $"{load.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE2":
                        att.TextString = $"{load.PowerCable.SizeTag}";
                        break;
                }
            }

        }
    }
    
    private void InsertMccBus(IDteq mcc, double[] insertionPoint, double blockSpacing)
    {
        double[] linePoint1 = new double[3];
        linePoint1[0] = -1.5;
        linePoint1[1] = 0;
        double[] linePoint2 = new double[3];
        linePoint2[0] = blockSpacing * mcc.AssignedLoads.Count + 1;
        linePoint2[1] = 0;

        AcadLine busLine = _acad.AcadDoc.ModelSpace.AddLine(linePoint1, linePoint2);
        busLine.Layer = "ECT_CONN_GENERAL_WIRES";
    }
    private void InsertMccBorder(IDteq mcc, double[] insertionPoint, double blockSpacing, double Xscale = 1, double Yscale = 1, double Zscale = 1)
    {
        string blockPath = EdtSettings.AcadBlockFolder + @"\Single Line\MCC_BORDER.dwg";
        double borderScale = blockSpacing * (mcc.AssignedLoads.Count + 2.5);
        insertionPoint[0] = 0-2;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;
        var border = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, borderScale, Yscale, Zscale, 0);
    }


}
