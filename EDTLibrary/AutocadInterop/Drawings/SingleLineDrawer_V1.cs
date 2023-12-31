﻿using AutocadLibrary;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Settings;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Printing;
using System.Threading;

namespace EDTLibrary.Autocad.Interop;
public class SingleLineDrawer_V1 : ISingleLineDrawer
{

    public SingleLineDrawer_V1(AutocadHelper acadService)

    {
        AcadHelper = acadService;
    }

    public AutocadHelper AcadHelper { get => _acadHelper; set => _acadHelper = value; }
    private AutocadHelper _acadHelper;
    public string BlockSourceFolder { get { return EdtProjectSettings.AcadBlockFolder; } }


    public double[] _insertionPoint = new double[3];
    int firstLoadSpacing = 1;

    public void DrawSingleLine(IDteq mcc, double blockSpacing = 1.5)
    {

        try {

            InsertMainBlock(mcc, _insertionPoint, "Breaker");

            _insertionPoint[0] += firstLoadSpacing;


            InsertMccLoads(mcc, blockSpacing);

            InsertMccBus(mcc, _insertionPoint, blockSpacing);
            InsertMccBorder(mcc, _insertionPoint, blockSpacing);
        }
        catch (Exception) {

            throw;
        }

    }

    private void InsertMccLoads(IDteq mcc, double blockSpacing)
    {
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
    }

    private void InsertMainBlock(IDteq mcc, double[] insertionPoint, string blockType = "Default")
    {
        //Instert Main Block
        insertionPoint[0] = 0;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;

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

        var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);

        var blockAtts = (dynamic)acadBlock.GetAttributes();

        foreach (dynamic att in blockAtts) {
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
        //string blockName = "SL_" + blockType + ".dwg";
        string blockName = "SL" + "_DCN" + ".dwg";
        string blockPath = sourcePath + blockName;


        //instert block
        var acadBlock = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, Xscale, Yscale, Zscale, 0);
        //acadBlock.Modified += AcadEventHandler.OnAcadModified;
        AcadEventHandler.raisers.Add(acadBlock);


        //Set Dynamic Block Propterties
        dynamic blockProps = acadBlock.GetDynamicBlockProperties();

        if (load.DisconnectBool && load.Disconnect.Type.Contains("FDS")) {
            foreach (dynamic blockProp in blockProps) {
                if (blockProp.PropertyName == "Disconnect Type") {
                    blockProp.Value = "Fused";
                }
            }
        }

        dynamic blockAtts = acadBlock.GetAttributes();

        //Set Text Attributes
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
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.Tag}";
                    break;
                case "FrameAmps":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.FrameAmps} AF";
                    break;

                case "TripAmps":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.TripAmps} AT";
                    break;

                case "FrameAmps2":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.FrameAmps} AF";
                    break;



                //Cable 1 V1
                case "CABLE_TAG":
                    att.TextString = $"{load.PowerCable.Tag}";
                    break;
                case "CABLE_SIZE":
                    att.TextString = $"{load.PowerCable.SizeTag}";
                    break;

                //Breaker V1
                case "LOAD_SIZE":
                    att.TextString = load.Type == LoadTypes.MOTOR.ToString() ? $"{load.Size}" : $"{load.Size} {load.Unit}";
                    break;
                case "LOAD_TAG":
                    att.TextString = $"{load.Tag}";
                    break;
                case "LOAD_DESCRIPTION":
                    att.TextString = $"{load.Description}";
                    break;

                //StandAloneStarter V1
                case "DRIVE_TAG":
                    if (load.StandAloneStarter != null)
                        att.TextString = $"{load.StandAloneStarter.Tag}";
                    break;
                case "DRIVE_TYPE":
                    if (load.StandAloneStarter != null)
                        att.TextString = $"{load.StandAloneStarter.Type}";
                    break;


                //Disconnect V1
                case "DCN_TAG":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.Tag}";
                    break;
                case "DCN_SIZE":
                    if (load.Disconnect != null)
                        att.TextString = $"{load.Disconnect.FrameAmps}";
                    break;
            }

            //Disconnect and StandAloneStarter
            if (load.DisconnectBool == true && load.StandAloneStarterBool == true && isDriveInternal == false) {
                switch (att.TagString) {
                    case "CABLE_TAG1":
                        att.TextString = $"{load.StandAloneStarter.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE1":
                        att.TextString = $"{load.StandAloneStarter.PowerCable.SizeTag}";
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
            //StandAloneStarter only
            else if (load.DisconnectBool == false && load.StandAloneStarterBool == true && isDriveInternal == false) {
                switch (att.TagString) {
                    case "CABLE_TAG1":
                        att.TextString = $"{load.StandAloneStarter.PowerCable.Tag}";
                        break;
                    case "CABLE_SIZE1":
                        att.TextString = $"{load.StandAloneStarter.PowerCable.SizeTag}";
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
        double[] lineStart = new double[3];
        lineStart[0] = -1.5;
        lineStart[1] = 0;
        double[] lineEnd = new double[3];
        lineEnd[0] = blockSpacing * mcc.AssignedLoads.Count + 1;
        lineEnd[1] = 0;

        dynamic busLine = AcadHelper.AcadDoc.ModelSpace.AddLine(lineStart, lineEnd);
        busLine.Layer = "ECT_CONN_GENERAL_WIRES";
    }
    private void InsertMccBorder(IDteq mcc, double[] insertionPoint, double blockSpacing, double Xscale = 1, double Yscale = 1, double Zscale = 1)
    {
        string blockPath = EdtProjectSettings.AcadBlockFolder + @"\Single Line\MCC_BORDER.dwg";
        double borderScale = blockSpacing * (mcc.AssignedLoads.Count + 2.5);
        insertionPoint[0] = 0 - 2;
        insertionPoint[1] = 0;
        insertionPoint[2] = 0;
        dynamic border = AcadHelper.AcadDoc.ModelSpace.InsertBlock(insertionPoint, blockPath, borderScale, Yscale, Zscale, 0);

    }


}
