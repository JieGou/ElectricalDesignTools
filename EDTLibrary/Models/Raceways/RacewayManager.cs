using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Loads;
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

    public static async Task<RacewayModel> AddRaceway(object racewayToAddObject, ListManager listManager)
    {
        var racewayToAddValidator = (RacewayToAddValidator)racewayToAddObject;
        var IsValid = racewayToAddValidator.IsValid();
        var errors = racewayToAddValidator._errorDict;
        if (IsValid == false) return null;

        var newRaceway = new RacewayModel();
        newRaceway.Tag = racewayToAddValidator.Tag;
        newRaceway.Category = Categories.RACEWAY.ToString();
        newRaceway.Type = racewayToAddValidator.Type;
        newRaceway.Material = racewayToAddValidator.Material;
        if (racewayToAddValidator.Height != null) newRaceway.Height = double.Parse(racewayToAddValidator.Height);
        if (racewayToAddValidator.Width != null) newRaceway.Width = double.Parse(racewayToAddValidator.Width);
        if (racewayToAddValidator.Diameter != null) newRaceway.Diameter = double.Parse(racewayToAddValidator.Diameter);
        
        newRaceway.Id = DaManager.prjDb.InsertRecordGetId(newRaceway, GlobalConfig.RacewayTable, NoSaveLists.RacewayNoSaveList);
        newRaceway.PropertyUpdated += DaManager.OnRacewayPropertyUpdated;
        listManager.RacewayList.Add(newRaceway);
        return newRaceway;

    }
}
