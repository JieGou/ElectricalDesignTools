using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.LibraryData.TypeModels;
public class LcsTypeModel : UserEditableTypeBase
{
    private string _type;

    public string Type
    {
        get => _type;
        set
        {
            if (string.IsNullOrEmpty(value.ToString())) return;
            _type = value;
        }
    }
    public string Description { get; set; }

    [Display(Name = "Digital Conductor Qty", ShortName = "Digital Conductor Qty")]
    public int DigitalConductorQty { get; set; }
    [DisplayName("Analog Conductor Qty")]
    public int AnalogConductorQty { get; set; }

}
