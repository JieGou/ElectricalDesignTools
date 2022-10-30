using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Raceways;
public class RacewayRouteSegment
{
    public int Id { get; set; }
    public int RacewayId { get; set; }
    public RacewayModel RacewayModel { get; set; }
    public int CableId { get; set; }
    public int SequenceNumber { get; set; }
}
