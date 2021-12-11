using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfUI.ViewModels;

namespace WpfUI.ValidationRules {
    public class InvalidFedFromRule : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {

            DteqModel Dteq = (value as BindingGroup).Items[0] as DteqModel;

            if (CheckCircularReference(Dteq, Dteq.FedFrom)) {
                return new ValidationResult(false, "Equipment cannot be fed from itself");
            }

            return ValidationResult.ValidResult;
        }
         
        private bool CheckCircularReference(DteqModel startDteq, string nextDteq, int counter =1) {
            var dteqDict = Dictionaries.dteqDict;
            if (nextDteq == null) return false;
            if (nextDteq == "" & counter == 1) { // sets the initial FedFrom
                nextDteq = startDteq.FedFrom;
            }

            if (dteqDict.ContainsKey(nextDteq) == false || nextDteq == "" || counter > dteqDict.Count) { // all equipment has been checked
                return false;
            }
            else if (startDteq.FedFrom == startDteq.Tag) { // if the equipment is fed from itself
                return true;
            }
            else if (dteqDict.ContainsKey(nextDteq)) {
                if (dteqDict[nextDteq].FedFrom == startDteq.Tag) { // if the equipment is indirectly fed from itself
                    return true;
                }
                counter += 1;
                return CheckCircularReference(startDteq, dteqDict[nextDteq].FedFrom, counter); // if not increase the count and check the next Tag
            }
            
            return false;
        }

       

    }
}
