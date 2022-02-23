using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Validators
{
    public  class ValidationHelper
    {
        public static void Revalidate(object obj)
        {
            string temp = "";
            var props = obj.GetType().GetProperties();
            foreach (var property in props) {

                if (property.GetSetMethod() != null) {
                    temp = (string)property.GetValue(obj);
                    property.SetValue(obj, "fake");
                    property.SetValue(obj, temp);
                }
            }


        }
    }
}
