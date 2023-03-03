using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Calculations;
public class CalculationLock
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public bool IsLocked { get; set; }
}
