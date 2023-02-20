using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Services;
using EDTLibrary.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace EDTLibrary.Validators
{
    public static class TagAndNameValidator
    {
        public static bool IsTagAvailable(string tagToCheck, ListManager listManager, bool showAlert = true)
        {
            if (string.IsNullOrEmpty(tagToCheck)) return true;
            if (listManager == null) return true; //for test data
            if (DaManager.GettingRecords == true) return true;
            if (tagToCheck == GlobalConfig.UtilityTag) return true;
            if (tagToCheck == GlobalConfig.Deleted) return true;
            if (tagToCheck == GlobalConfig.LargestMotor_StartLoad) return true;

            listManager.CreateEquipmentList();

            string tag = null;
            foreach (var eq in listManager.EqList)
            {
                if (eq.Tag != null)
                {
                    if (eq.Tag.ToLower() == tagToCheck.ToLower()) tag = eq.Tag;
                }
            }

            if (tag != null)
            {
                if (showAlert == true)
                {
                    EdtNotificationService.SendAlert(tagToCheck,
                                                     $"{ErrorMessages.DuplicateTagMessage}", 
                                                    "Duplicate Tag Error");
                }
                return false;
            }

            return true;
        }

        public static bool IsNameAvailable(string name, ListManager listManager)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            var areaNAme = listManager.AreaList.FirstOrDefault(t => t.Name.ToLower() == name.ToLower());

            if (areaNAme != null)
            {
                return false;
            }

            return true;
        }
    }
}
