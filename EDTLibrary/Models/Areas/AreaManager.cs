﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Areas;
public class AreaManager
{
    public static void UpdateArea(IEquipment caller, IArea newArea, IArea oldArea)
    {
        if (GlobalConfig.GettingRecords==false) {
            if (oldArea != null) {
                oldArea.PropertyChanged -= caller.OnAreaPropertiesChanged;
            }
            newArea.PropertyChanged += caller.OnAreaPropertiesChanged;
            caller.Area = newArea;
            caller.AreaId = newArea.Id;
            caller.NemaRating = newArea.NemaRating;
            caller.AreaClassification = newArea.AreaClassification;
        }
    }
}