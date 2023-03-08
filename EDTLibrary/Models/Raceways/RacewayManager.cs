using EdtLibrary.Settings;
using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Services;
using EDTLibrary.Settings;
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
        segmentToAdd.SequenceNumber = cable.RacewaySegmentList.Count;

        var segmentAlreadyAdded = cable.RacewaySegmentList.FirstOrDefault(rrs => rrs.RacewayModel.Id == raceway.Id);
        if (segmentAlreadyAdded == null) {
            cable.RacewaySegmentList.Add(segmentToAdd);
            listManager.RacewaySegmentList.Add(segmentToAdd);
            segmentToAdd.Id = DaManager.prjDb.InsertRecordGetId(segmentToAdd, "RacewayRouteSegments", new List<string> { "RacewayModel" });
        }
        else {
            EdtNotificationService.SendAlert(cable, $"Cable {cable.Tag} already passes through raceway {raceway.Tag}.", "Raceway Routing Conflict", 
                nameof(EdtAppSettings.Default.Notification_CableAlreadyInRaceway));
        }
        
    }

    public static void RemoveRacewayRouteSegment(RacewayRouteSegment segment, ListManager listManager)
    {
        var cable = listManager.CableList.FirstOrDefault(c => c.Id == segment.CableId);
        var segmentToRemove = listManager.RacewaySegmentList.FirstOrDefault(s => s.Id ==segment.Id); 
        cable.RacewaySegmentList.Remove(cable.RacewaySegmentList.FirstOrDefault(s => s.Id == segmentToRemove.Id));
        listManager.RacewaySegmentList.Remove(segmentToRemove);
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

    public static async Task<int> DeleteRaceway(object racewayToDeleteObject, ListManager listManager)
    {
        try {

            var racewayToDelete = (RacewayModel)racewayToDeleteObject;
            racewayToDelete.PropertyUpdated -= DaManager.OnRacewayPropertyUpdated;
            listManager.RacewayList.Remove(racewayToDelete);
            racewayToDelete.PropertyUpdated -= DaManager.OnLoadPropertyUpdated;
            DaManager.DeleteRaceway(racewayToDelete);

            //Delete segments from SegmentList and ListManager
            RacewayRouteSegment segmentToDelete = new RacewayRouteSegment();
            foreach (var cable in listManager.CableList) {
                segmentToDelete = cable.RacewaySegmentList.FirstOrDefault(rrs => rrs.RacewayId == racewayToDelete.Id);
                if (segmentToDelete != null) {
                    cable.RacewaySegmentList.Remove(segmentToDelete);
                }
            }

            var segmentsToDelete = new List<RacewayRouteSegment>();

            foreach (var segment in listManager.RacewaySegmentList) {
                if (segment.RacewayId == racewayToDelete.Id) {
                    segmentsToDelete.Add(segment);
                }
            }

            foreach (var segment in segmentsToDelete) {
                listManager.RacewaySegmentList.Remove(segment);
                DaManager.prjDb.DeleteRecord(GlobalConfig.RacewayRouteSegmentsTable, segment.Id);
            }
           
            return racewayToDelete.Id;
            
        }
        catch (Exception ex) {
            throw;
        }
        return -1;

    }
}
