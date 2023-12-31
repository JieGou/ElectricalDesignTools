﻿using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.Autocad.Interop;
public static class AcadEventHandler
{
    public static List<object> raisers = new List<object>();
    public static List<string> entities = new List<string>();
    public static void OnAcadModified(dynamic acadObject)
    {
        try {
            ErrorHelper.Log("Event fired", "AcadEventHandler");

            string objName = acadObject.ObjectName;
            dynamic block;
            object[] atts = new dynamic[0];

            var doc = (dynamic)acadObject.Document;
            foreach (dynamic item in doc.ModelSpace) {
                if (item == null) continue;
                entities.Add(item.EntityName);
            }
            foreach (dynamic item in doc.ModelSpace) {
                if (item == null) continue;

                if (acadObject.ObjectID == item.ObjectID) {
                    if (acadObject.ObjectName.Contains("Block")) {
                        block = (dynamic)item;
                        atts = (dynamic)block.GetAttributes();
                        break;
                    }

                    if (acadObject.ObjectName.Contains("Attribute")) {
                        block = (dynamic)item;
                        atts = (dynamic)block.GetAttributes();
                        break;
                    }
                }
            }

            string tag = "";
            string desc = "";

            if (atts.Count() > 0) {
                foreach (dynamic att in atts) {
                    if (att.TagString == "LOAD_TAG") {
                        tag = att.TextString;
                    }
                    if (att.TagString == "LOAD_DESCRIPTION") {
                        desc = att.TextString;

                    }
                }
            }
            var load = ScenarioManager.ListManager.LoadList.FirstOrDefault(l => l.Tag == tag);
            if (load != null) {
                load.Description = desc;
            }

            foreach (dynamic item in atts) {
                Debug.Print(item.TagString);
            }
        }
        catch (Exception ex) {
            ErrorHelper.Log(ex.Message, "AcadEventHandler");
            throw;
        }
    }

    //internal static IAcadObjectEvents_ModifiedEventHandler OnObjectModified(AcadObject modifiedObjct)
    //{
    //    return null;
    //}
}
