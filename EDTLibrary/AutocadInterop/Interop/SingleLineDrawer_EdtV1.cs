using AutocadLibrary;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;
using System.Threading;
using System.Windows;

namespace EDTLibrary.Autocad.Interop;
public class SingleLineDrawer_EdtV1 : ISingleLineDrawer
{

    public SingleLineDrawer_EdtV1(AutocadHelper acadHelper)

    {
        AcadHelper = acadHelper;
    }

    public AutocadHelper AcadHelper { get => _acadHelper; set => _acadHelper = value; }
    private AutocadHelper _acadHelper;

    public string BlockSourceFolder { get { return EdtProjectSettings.AcadBlockFolder; } }


    public double[] _insertionPoint = new double[3];
    int firstLoadSpacing = 1;

    double startX = 5;
    double startY = 11;

    public void DrawMccSingleLine(IDteq mcc, double blockSpacing = 1.5)
    {
        _insertionPoint[0] = startX;
        _insertionPoint[1] = startY;
        _insertionPoint[2] = 0;

        try {

            InsertMainBlock(mcc, _insertionPoint, "Breaker");

            _insertionPoint[0] += firstLoadSpacing;


            InsertMccLoads(mcc, blockSpacing);

            InsertMccBus(mcc, _insertionPoint, blockSpacing);
            InsertMccBorder(mcc, _insertionPoint, blockSpacing);

            AcadDocument doc;
            AcadHelper.AcadDoc.SendCommand("ATTSYNC N MCC_FCB" + Environment.NewLine);
            AcadHelper.AcadDoc.SendCommand("ATTSYNC N MCC_DOL" + Environment.NewLine);
        }
        catch (Exception) {

            throw;
        }

    }

    private void InsertMccLoads(IDteq mcc, double blockSpacing)
    {

        foreach (var powerConsumer in mcc.AssignedLoads) {

                var load = (IPowerConsumer)powerConsumer;

                _insertionPoint[1] = startY;

                InsertLoadBucket(load, _insertionPoint);

                InsertLoad(load, _insertionPoint);

                //check if graphic is wide and move next load over to make room for this wide block
                if (load.DisconnectBool == true && load.StandAloneStarterBool == true) {
                    _insertionPoint[0] += .5;
            }

            _insertionPoint[0] += blockSpacing;
        }
    }

    private void InsertMainBlock(IDteq mcc, double[] insertionPoint, string blockType = "Default")
    {
        //Instert Main Block

        blockType = blockType == "Default" ? "Breaker" : blockType;
        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "MCC_MAIN_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        double Xscale = 1;
        double Yscale = 1;
        double Zscale = 1;

        Thread.Sleep(1000);


        var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        //acadBlock.Modified += AcadEventHandler.OnAcadModified;
        AcadEventHandler.raisers.Add(acadBlock);

        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (dynamic att in blockAtts) {
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
    private void InsertLoadBucket(IPowerConsumer load, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1, bool isDriveInternal = false)
    {

        string blockType = "FCB";


        if (load.Type == LoadTypes.MOTOR.ToString() 
            && load.ProtectionDevice.Type.Contains("VSD") == false
            && load.ProtectionDevice.Type.Contains("VFD") == false
            && load.ProtectionDevice.Type.Contains("RVS") == false) 
        {
            blockType = "DOL";
        }
        

        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "MCC_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;

        var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);



        //Set Dynamic Block Propterties
        dynamic blockProps = acadBlock.GetDynamicBlockProperties();
        foreach (dynamic blockProp in blockProps) {


            // Starter Bucket Type
            string visbilityState = "BiMetallic"; //EdtProjectSettings.OverloadGraphicType;

            if (blockProp.PropertyName == "Starter Bucket Type") {
                if (load.ProtectionDevice.Type.Contains("FVR")) {
                    visbilityState += " Reversing";
                }
                blockProp.Value = visbilityState;
            }

            //FCB Bucket Type
            visbilityState = "Breaker"; //EdtProjectSettings.OverloadType;

            if (blockProp.PropertyName == "FCB Bucket Type") {
                if (false) { //ProtectionDevice.HasGroundFaultMonitor
                    blockProp.Value = "Breaker GFM";
                }
                else if (load.ProtectionDevice.Type == ("VSD") 
                     || load.ProtectionDevice.Type == ("VFD") 
                     || load.ProtectionDevice.Type == ("RVS")) 
                {
                    blockProp.Value = "Drive";
                    if (false) { //load.ProtectionDevice.HasBypassContactor
                        blockProp.Value = "Drive Bypass";
                    }
                }
               
            }

        }

        //Set Text Attributes
        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (dynamic att in blockAtts) {
            switch (att.TagString) {


                //Starter
                case "MCP_Size":
                    att.TextString = $"{load.ProtectionDevice.TripAmps} A";
                    break;
                case "DOL_Size":
                    att.TextString = $"{load.ProtectionDevice.StarterSize}";
                    break;
                case "GFMonitor":
                    att.TextString = $"GFM"; //EdtSettings.GroundFaultMonitorType
                    break;


                //Breaker
                case "TripAmps":
                    att.TextString = $"{load.ProtectionDevice.TripAmps} AT";
                    break;
                case "FrameAmps":
                    att.TextString = $"{load.ProtectionDevice.FrameAmps} AF";
                    break;


                //StandAloneStarter
                case "DRIVE_TYPE":
                    if (load.StandAloneStarter != null)
                        att.TextString = $"{load.StandAloneStarter.Type}";
                    break;
                case "DRIVE_TAG":
                    if (load.StandAloneStarter != null && isDriveInternal == false)
                        att.TextString = $"";
                    break;

            }

        }

        //Set LoadSide Insertion Point
        AcadAttributeReference attr;
        foreach (dynamic att in blockAtts) {
            if (att.TagString == "LoadConnectionPoint") {
                insertionPoint[1] = att.InsertionPoint[1];
            }
        }

    }

    private void InsertLoadComponents(IPowerConsumer load, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1, bool isDriveInternal = false)
    {

        //default Load
        string blockType = "_DCN";
        var tag = load.Tag;

        //select component type
        foreach (var comp in load.CctComponents) {

            //Disconnect
            if (comp.SubCategory == CctCompSubCategories.Disconnect.ToString()) {
                blockType = "_Disconnect";
            }

            //VSF, VFD, RVS
            else if (comp.Type == ("VSD") || comp.Type == ("VFD") || comp.Type == ("RVS")) {
                blockType = "_Drive";
            }



            string sourcePath = BlockSourceFolder + @"\Single Line\";
            string blockName = "SL" + blockType + ".dwg";
            string blockPath = sourcePath + blockName;


            //instert block
            var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
            //acadBlock.Modified += AcadEventHandler.OnAcadModified;
            AcadEventHandler.raisers.Add(acadBlock);


            //Set Dynamic Block Propterties
            dynamic blockProps = acadBlock.GetDynamicBlockProperties();
            foreach (dynamic blockProp in blockProps) {

                //Disconnect Type
                if (blockProp.PropertyName == "Disconnect Type") {
                    if (comp.Type.Contains("FDS") || comp.Type.Contains("Fused")) {
                        blockProp.Value = "Fused";
                    }
                    else {
                        blockProp.Value = "Unfused";
                    }
                }

                //Drive Filters
                if (blockProp.PropertyName == "Line and Load Filters") {
                    if (true) {
                        blockProp.Value = "None";
                    }
                    else if ("Line" == "Line") {
                        blockProp.Value = "Line Reactor";

                    }
                    else if ("Load" == "Load") {
                        blockProp.Value = "Line Filter";

                    }
                    else if ("Both" == "Both") {
                        blockProp.Value = "Both";

                    }
                }

            }
            

            //Set Text Attributes
            dynamic blockAtts = acadBlock.GetAttributes();
            foreach (dynamic att in blockAtts) {
                switch (att.TagString) {

                    //Edt Cable
                    case "Cable_Tag":
                        att.TextString = $"{load.PowerCable.Tag}";
                        break;
                    case "Cable_Size":
                        att.TextString = $"{load.PowerCable.SizeTag}";
                        break;



                    //Edt Disconnect
                    case "Disconnect_Tag":
                        att.TextString = $"{comp.Tag}";
                        break;
                    case "FrameAmps":
                        att.TextString = $"{comp.FrameAmps} AF";
                        break;

                    case "TripAmps":
                        att.TextString = $"{comp.TripAmps} AT";
                        break;


                    //Edt Drive
                    case "Drive_Tag":
                        att.TextString = $"{comp.Tag}";
                        break;
                    case "Drive_Type":
                        att.TextString = $"{comp.Type}";
                        break;
                }
            }


            //Get LoadSide Insertion Point
            AcadAttributeReference attr;
            foreach (dynamic att in blockAtts) {
                if (att.TagString == "LoadConnectionPoint") {
                    insertionPoint[1] = att.InsertionPoint[1];
                }
            }
        }

    }
    private void InsertLoad(IPowerConsumer load, double[] insertionPoint, double Xscale = 1, double Yscale = 1, double Zscale = 1, bool isDriveInternal = false)
    {
        InsertLoadComponents(load, insertionPoint);
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

      
        string sourcePath = BlockSourceFolder + @"\Single Line\";
        string blockName = "SL_" + blockType + ".dwg";
        string blockPath = sourcePath + blockName;


        //insert block
        var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        //acadBlock.Modified += AcadEventHandler.OnAcadModified;
        AcadEventHandler.raisers.Add(acadBlock);

        dynamic blockAtts = acadBlock.GetAttributes();

        //Set Text Attributes
        foreach (dynamic att in blockAtts) {
            switch (att.TagString) {

                //Cable 1 V1
                case "CABLE_TAG":
                    att.TextString = $"{load.PowerCable.Tag}";
                    break;
                case "CABLE_SIZE":
                    att.TextString = $"{load.PowerCable.SizeTag}";
                    break;

                //Load V1
                case "LOAD_SIZE":
                    att.TextString = load.Type == LoadTypes.MOTOR.ToString() ? $"{load.Size}" : $"{load.Size} {load.Unit}";
                    break;
                case "LOAD_TAG":
                    att.TextString = $"{load.Tag}";
                    break;
                case "LOAD_DESCRIPTION":
                    att.TextString = $"{load.Description}";
                    break;

            }

            insertionPoint[1] = startY;

        }
    }
    
    
    private void InsertMccBus(IDteq mcc, double[] insertionPoint, double blockSpacing)
    {
        double[] lineStart = new double[3];
        lineStart[0] = startX - 0.5;
        lineStart[1] = startY;
        double[] lineEnd = new double[3];
        lineEnd[0] = startX + blockSpacing * mcc.AssignedLoads.Count + 1;
        lineEnd[1] = startY;

        dynamic busLine = AcadHelper.AcadDoc.ModelSpace.AddLine(lineStart, lineEnd);
        busLine.Layer = "ECT_CONN_GENERAL_WIRES";
    }
    private void InsertMccBorder(IDteq mcc, double[] insertionPoint, double blockSpacing, double Xscale = 1, double Yscale = 1, double Zscale = 1)
    {
        string blockPath = EdtProjectSettings.AcadBlockFolder + @"\Single Line\MCC_BORDER.dwg";
        double borderScale = blockSpacing * (mcc.AssignedLoads.Count + 2.5);
        insertionPoint[0] = startX - 2;
        insertionPoint[1] = startY;
        insertionPoint[2] = 0;
        dynamic border = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, 1, 1, 1, 0);

        //Set Dynamic Block Propterties
        dynamic blockProps = border.GetDynamicBlockProperties();
        foreach (dynamic blockProp in blockProps) {

            //Disconnect Type
            if (blockProp.PropertyName == "Horizontal") {
                blockProp.Value = borderScale;
            }

        }
    }


}
