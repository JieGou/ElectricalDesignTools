using AutoCAD;
using AutocadLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;

namespace EDTLibrary.Autocad.Interop;
public class SingleLineDrawer
{

    public SingleLineDrawer(AutocadHelper acadService, string blockSourceFolder)

    {
        _acad = acadService;
        BlockSourceFolder = blockSourceFolder;
    }

    private AutocadHelper _acad;
    public string BlockSourceFolder { get; }

    public double[] _insertionPoint = new double[3];
    int firstLoadSpacing = 1;

    public void DrawMccSingleLine(IDteq mcc, double blockSpacing = 1.5)
    {

        try {
            //if (_acad.AcadDoc == null) {
            //    _acad.AddDrawing();
            //}
            InsertMainBlock(mcc, _insertionPoint, "BKR");

            _insertionPoint[0] += firstLoadSpacing;


            foreach (var powerConsumer in mcc.AssignedLoads) {

                if (powerConsumer.GetType() == typeof(LoadModel)) {
                    var load = (LoadModel)powerConsumer;

                    InsertLoadBucket(load, _insertionPoint);

                    _insertionPoint[1] -= 1.65;
                    InsertLoad(load, _insertionPoint);
                    _insertionPoint[1] += 1.65;
                    //check if graphic is wide and move next load over to make room for this wide block
                    if (load.DisconnectBool == true && load.StandAloneStarterBool == true) {
                        _insertionPoint[0] += .5;
                    }
                }

                _insertionPoint[0] += blockSpacing;
            }

            InsertMccBus(mcc, _insertionPoint, blockSpacing);
            InsertMccBorder(mcc, _insertionPoint, blockSpacing);
        }
        catch (Exception) {

            throw;
        }
   
    }

    private void InsertMainBlock(IDteq mcc, double[] insertionPoint, string blockType = "Default")
    {
        //Instert Main Block
        insertionPoint[0] = 0;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;

        blockType = blockType == "Default" ? "BKR" : blockType;
        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "MCC_MAIN_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        
        var acadBlock = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        acadBlock.Modified += AcadEventHandler.OnAcadModified;
        AcadEventHandler.raisers.Add(acadBlock);

        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (AcadAttributeReference att in blockAtts) {
            switch (att.TagString) {
                case "BUS_DETAILS":
                    att.TextString = $"{mcc.LineVoltage} V, {mcc.Size} A, 3-PH, {mcc.SCCR} kA";
                    break;
                case "AF":
                    att.TextString = $"{mcc.ProtectionDevice.FrameAmps} AF";
                    break;
                case "AT":
                    att.TextString = $"{mcc.ProtectionDevice.TripAmps} AT";
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
    }
    private void InsertLoadBucket(LoadModel load, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1, bool isDriveInternal = false)
    {

        string blockType = "FCB";

        if (load.ProtectionDevice.Type.Contains("FVNR")) {
            blockType = "FVNR";
        }
        else if (load.ProtectionDevice.Type.Contains("FVR")) {
            blockType = "FVR";
        }
        else if (load.StandAloneStarterBool == true && isDriveInternal == true) {
            blockType = "DRIVE";
        }

        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "MCC_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        var acadBlock = _acad.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        
        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (AcadAttributeReference att in blockAtts) {
            switch (att.TagString) {

                //Starter
                case "MCP_Size":
                    att.TextString = $"{load.ProtectionDevice.TripAmps} A";
                    break;
                case "FVNR_Size":
                    att.TextString = $"{load.ProtectionDevice.StarterSize}";
                    break;
                case "FVR_Size":
                    att.TextString = $"{load.ProtectionDevice.StarterSize}";
                    break;

                //Breaker
                case "AT":
                    att.TextString = $"{load.ProtectionDevice.TripAmps}";
                    break;
                case "AF":
                    att.TextString = $"{load.ProtectionDevice.FrameAmps}";
                    break;

                //Drive
                case "DRIVE_TYPE":
                    if (load.Drive != null)
                        att.TextString = $"{load.Drive.Type}";
                    break;
                case "DRIVE_TAG":
                    if (load.Drive != null && isDriveInternal == false) 
                        att.TextString = $""; 
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

        if (load.StandAloneStarterBool == true && isDriveInternal == false) {
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
        acadBlock.Modified += AcadEventHandler.OnAcadModified;
        AcadEventHandler.raisers.Add(acadBlock);

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
            if (load.DisconnectBool == true && load.StandAloneStarterBool == true && isDriveInternal == false) {
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
            else if (load.DisconnectBool == true && load.StandAloneStarterBool == false) {
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
            else if (load.DisconnectBool == false && load.StandAloneStarterBool == true && isDriveInternal==false) {
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

            //att.Modified += AcadEventHandler.OnAcadModified;
            //AcadEventHandler.raisers.Add(att);
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
