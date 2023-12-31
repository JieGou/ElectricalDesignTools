﻿using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfUI.Stores;
using WpfUI.ViewModels;

namespace WpfUI.ValidationRules
{
    public class InvalidFedFromRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {

            IDteq Dteq = (value as BindingGroup).Items[0] as IDteq;

            if (CheckFedFromSelf(Dteq, Dteq.FedFromTag)) {
                return new ValidationResult(false, "Equipment cannot be fed from itself");
            }

            return ValidationResult.ValidResult;
        }
         
        //TODO - fix this validation rule
        private bool CheckFedFromSelf(IDteq startDteq, string nextDteq, int counter =1) {
            var dteqDict = DictionaryStore.dteqDict;
            if (nextDteq == null) return false;
            if (nextDteq == "" & counter == 1) { // sets the initial FedFrom
                nextDteq = startDteq.FedFromTag;
            }

            if (dteqDict.ContainsKey(nextDteq) == false || nextDteq == "" || counter > dteqDict.Count) { // all equipment has been checked
                return false;
            }
            else if (startDteq.FedFromTag == startDteq.Tag) { // if the equipment is fed from itself
                return true;
            }
            else if (dteqDict.ContainsKey(nextDteq)) {
                if (dteqDict[nextDteq].FedFromTag == startDteq.Tag) { // if the equipment is indirectly fed from itself
                    return true;
                }
                counter += 1;
                return CheckFedFromSelf(startDteq, dteqDict[nextDteq].FedFromTag, counter); // if not increase the count and check the next Tag
            }
            
            return false;
        }
    }
}
