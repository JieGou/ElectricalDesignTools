using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Raceways;
public class RacewayManager
{
    public static void AddRacewayRouteSegment(RacewayModel raceway, ICable cable, ListManager listManager)
    {
        var segmentToAdd = new RacewayRouteSegment();
        segmentToAdd.RacewayId = raceway.Id;
        segmentToAdd.RacewayModel = raceway;
        segmentToAdd.CableId = cable.Id;
        segmentToAdd.SequenceNumber = cable.RacewayRouteSegments.Count;

        var segmentAlreadyAdded = cable.RacewayRouteSegments.FirstOrDefault(rrs => rrs.RacewayModel.Id == raceway.Id);
        if (segmentAlreadyAdded == null) {
            cable.RacewayRouteSegments.Add(segmentToAdd);
            listManager.RacewayRoutingList.Add(segmentToAdd);
            DaManager.prjDb.InsertRecordGetId(segmentToAdd, "RacewayRouteSegments", new List<string> { "RacewayModel" });
        }
        else {
            EdtNotificationService.SendAlert(cable, "Cable already passes through the selected raceway.", "Raceway Route Error");
        }
        
    }

    public static void RemoveRacewayRouteSegment(RacewayRouteSegment segment, ICable cable, ListManager listManager)
    {
        cable.RacewayRouteSegments.Remove(segment);
        listManager.RacewayRoutingList.Remove(segment);
        DaManager.prjDb.DeleteRecord("RacewayRouteSegments", segment.Id);
    }
}
