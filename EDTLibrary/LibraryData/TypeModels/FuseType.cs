using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData.TypeModels;
public class FuseType
{
    public int Id { get; set; }
    public double Type { get; set; }
    public double Description { get; set; }

    public double MaxCurrentRating { get; set; }
}
