﻿using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Services;
using EDTLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.ProjectSettings;
public class TagManager
{
    public static ObservableCollection<TagModel> TagSettingList { get; set; } = new ObservableCollection<TagModel>();
    public static void LoadTagSettings()
    {
        TagSettingList = DaManager.PrjDb.GetRecords<TagModel>("TagSettings");

        var tagSettingsClass = typeof(TagSettings);
        foreach (var tagSetting in TagSettingList) {

            foreach (var property in tagSettingsClass.GetProperties()) {
                if (tagSetting.Name == property.Name) {
                    property.SetValue(string.Empty, tagSetting.Value);
                }

            }
        }
    }

    public static void SaveSettingToDb(string settingName, string settingValue)
    {
        TagModel setting = new TagModel();
        setting = TagSettingList.FirstOrDefault(s => s.Name == settingName);
        setting.Value = settingValue;
        DaManager.PrjDb.UpdateRecord<TagModel>(setting, "TagSettings");
    }



    public static string AssignEqTag(IEquipment eq, ListManager listManager)
    {
        var identifier = GetEquipmentIdentifier(eq);
        var sequenceNumber = GetNextSequentialNumber(eq, listManager);
        
        var tag = identifier + TagSettings.EqIdentifierSeparator + sequenceNumber;

        if (TagAndNameValidator.IsTagAvailable(tag,listManager) == false) {

            do {
                sequenceNumber = (int.Parse(sequenceNumber) + 1).ToString();
                tag = identifier + TagSettings.EqIdentifierSeparator + sequenceNumber;
            } while (TagAndNameValidator.IsTagAvailable(tag, listManager) == false);
        }
        
        return tag;
    }
    private static string GetNextSequentialNumber(IEquipment eq, ListManager listManager)
    {
        var sequenceNumber = "000";

        listManager.CreateEquipmentList();

        var eqTypelist = listManager.EqList.Where(e => e.Type == eq.Type).ToList();
        var eqNumList = new List<int>();

        int outTagNum = 0;
        if (eqTypelist.Count > 0) {
            foreach (var item in eqTypelist) {
                if (item.Tag == null) {
                   
                }
                else {
                    int.TryParse(item.Tag.Substring(item.Tag.IndexOf(TagSettings.EqIdentifierSeparator) + 1), out outTagNum);
                    eqNumList.Add(outTagNum); 
                }
            }

            if (eqNumList.Count > 0) {
                var maxNum = eqNumList.Max();
                sequenceNumber = (maxNum += 1).ToString(); 
            }
            else {
                sequenceNumber = "1";
            }
        }
        else {
            sequenceNumber = "1";
        }
        

        var length = int.Parse(TagSettings.EqNumberDigitCount) - sequenceNumber.Length;
        for (int i = 0; i < length; i++) {
            sequenceNumber = "0" + sequenceNumber;
        }

        return sequenceNumber;

    }
    private static string GetEquipmentIdentifier(IEquipment eq)
    {

        try {
            if (eq.Type == DteqTypes.XFR.ToString()) {
                var dteq = DteqFactory.Recast(eq);

                switch (dteq.LoadVoltageType.Voltage) {
                    case <= 240:
                        return TagSettings.LvTransformerIdentifier;
                        break;
                    case > 240:
                        return TagSettings.TransformerIdentifier;
                        break;
                }
                return "Unidentified XFR voltage Type" + nameof(GetEquipmentIdentifier);


            }
            else if (eq.Type == DteqTypes.SWG.ToString()) {
                return TagSettings.SwgIdentifier;
            }
            else if (eq.Type == DteqTypes.MCC.ToString()) {
                return TagSettings.MccIdentifier;
            }
            else if (eq.Type == DteqTypes.CDP.ToString()) {
                return TagSettings.CdpIdentifier;
            }
            else if (eq.Type == DteqTypes.DPN.ToString()) {
                return TagSettings.DpnIdentifier;
            }
            else if (eq.Type == DteqTypes.SPL.ToString()) {
                return TagSettings.SplitterIdentifier;
            }

            //loads
            else if (eq.Type == LoadTypes.MOTOR.ToString()) {
                return TagSettings.MotorLoadIdentifier;
            }
            else if (eq.Type == LoadTypes.HEATER.ToString()) {
                return TagSettings.HeaterLoadIdentifier;
            }
            else if (eq.Type == LoadTypes.WELDING.ToString()) {
                return TagSettings.WeldingLoadIdentifier;
            }
            else if (eq.Type == LoadTypes.PANEL.ToString()) {
                return TagSettings.PanelLoadIdentifier;
            }
            else if (eq.Type == LoadTypes.OTHER.ToString()) {
                return TagSettings.OtherLoadIdentifier;
            }


            //Components
            else if (eq.Type.Contains(StarterTypes.DOL.ToString()) || eq.Type.Contains("MCP")) {
                return TagSettings.StarterSuffix;
            }
            else if (eq.Type.Contains(StarterTypes.VSD.ToString()) || eq.Type.Contains(StarterTypes.VFD.ToString())) {
                return TagSettings.DriveSuffix;
            }
            else if (eq.Type == CctComponentTypes.UDS.ToString() || eq.Type == CctComponentTypes.UDS.ToString()) {
                return TagSettings.DisconnectSuffix;
            }

            else {
                return "Unidentified Equipment Type - " + nameof(GetEquipmentIdentifier);
            }
        }
        catch (Exception ex) {
            return "Unidentified Equipment Type - " + nameof(GetEquipmentIdentifier);

            EdtNotificationService.SendError(nameof(GetEquipmentIdentifier), ex.Message, "Error", ex );
        }
    }
    public static string GetCableTypeIdentifier(ICable cable)
    {
        string output = "";


        if (cable.UsageType == CableUsageTypes.Power.ToString()) {
            output = TagSettings.PowerCableTypeIdentifier;
        }
        else if (cable.UsageType == CableUsageTypes.Control.ToString()) {
            output = TagSettings.ControlCableTypeIdentifier;
        }
        else if (cable.UsageType == CableUsageTypes.Instrument.ToString()) {
            output = TagSettings.InstrumentCableTypeIdentifier;
        }
        else if (cable.UsageType == CableUsageTypes.Ethernet.ToString()) {
            output = TagSettings.EthernetCableTypeIdentifier;
        }
        else if (cable.UsageType == CableUsageTypes.Fiber.ToString()) {
            output = TagSettings.FiberCableTypeIdentifier;
        }

        return output;
    }


    public static string AssignEqTag(string eqType, ListManager listManager)
    {
        var identifier = GetEquipmentIdentifier(eqType);
        var sequenceNumber = GetNextSequentialNumber(eqType, listManager);

        var tag = identifier + TagSettings.EqIdentifierSeparator + sequenceNumber;

        if (TagAndNameValidator.IsTagAvailable(tag, listManager) == false) {

            do {
                sequenceNumber = (int.Parse(sequenceNumber) + 1).ToString();
                tag = identifier + TagSettings.EqIdentifierSeparator + sequenceNumber;
            } while (TagAndNameValidator.IsTagAvailable(tag, listManager) == false);
        }

        return tag;
    }
    private static string GetNextSequentialNumber(string eqType, ListManager listManager)
    {
        var sequenceNumber = "000";

        listManager.CreateEquipmentList();

        var eqTypelist = listManager.EqList.Where(e => e.Type == eqType).ToList();
        var eqNumList = new List<int>();

        int outTagNum = 0;
        if (eqTypelist.Count > 0) {
            foreach (var item in eqTypelist) {
                if (item.Tag == null) {

                }
                else {
                    int.TryParse(item.Tag.Substring(item.Tag.IndexOf(TagSettings.EqIdentifierSeparator) + 1), out outTagNum);
                    eqNumList.Add(outTagNum);
                }
            }

            if (eqNumList.Count > 0) {
                var maxNum = eqNumList.Max();
                sequenceNumber = (maxNum += 1).ToString();
            }
            else {
                sequenceNumber = "1";
            }
        }
        else {
            sequenceNumber = "1";
        }


        var length = int.Parse(TagSettings.EqNumberDigitCount) - sequenceNumber.Length;
        for (int i = 0; i < length; i++) {
            sequenceNumber = "0" + sequenceNumber;
        }

        return sequenceNumber;

    }
    private static string GetEquipmentIdentifier(string eqType)
    {

        try {
            if (eqType == DteqTypes.XFR.ToString()) {
                return TagSettings.TransformerIdentifier;
            }
            else if (eqType == DteqTypes.SWG.ToString()) {
                return TagSettings.SwgIdentifier;
            }
            else if (eqType == DteqTypes.MCC.ToString()) {
                return TagSettings.MccIdentifier;
            }
            else if (eqType == DteqTypes.CDP.ToString()) {
                return TagSettings.CdpIdentifier;
            }
            else if (eqType == DteqTypes.DPN.ToString()) {
                return TagSettings.DpnIdentifier;
            }
            else if (eqType == DteqTypes.SPL.ToString()) {
                return TagSettings.SplitterIdentifier;
            }

            //loads
            else if (eqType == LoadTypes.MOTOR.ToString()) {
                return TagSettings.MotorLoadIdentifier;
            }
            else if (eqType == LoadTypes.HEATER.ToString()) {
                return TagSettings.HeaterLoadIdentifier;
            }
            else if (eqType == LoadTypes.WELDING.ToString()) {
                return TagSettings.WeldingLoadIdentifier;
            }
            else if (eqType == LoadTypes.PANEL.ToString()) {
                return TagSettings.PanelLoadIdentifier;
            }
            else if (eqType == LoadTypes.OTHER.ToString()) {
                return TagSettings.OtherLoadIdentifier;
            }


            //Components
            else if (eqType.Contains(StarterTypes.DOL.ToString()) || eqType.Contains("MCP")) {
                return TagSettings.StarterSuffix;
            }
            else if (eqType.Contains(StarterTypes.VSD.ToString()) || eqType.Contains(StarterTypes.VFD.ToString())) {
                return TagSettings.DriveSuffix;
            }
            else if (eqType == CctComponentTypes.UDS.ToString() || eqType == CctComponentTypes.UDS.ToString()) {
                return TagSettings.DisconnectSuffix;
            }

            else {
                return "Unidentified Equipment Type - " + nameof(GetEquipmentIdentifier);
            }
        }
        catch (Exception ex) {
            return "Unidentified Equipment Type - " + nameof(GetEquipmentIdentifier);

            EdtNotificationService.SendError(nameof(GetEquipmentIdentifier), ex.Message, "Error", ex);
        }
    }
}
