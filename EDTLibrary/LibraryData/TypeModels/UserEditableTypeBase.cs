using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdtLibrary.LibraryData.TypeModels;
public class UserEditableTypeBase
{
    public virtual int Id { get; set; }

    [DisplayName("Added by User")]
    public bool AddedByUser { get; set; }

    [DisplayName("Date Added")]
    public DateTime LastEdited { get; set; }
}
