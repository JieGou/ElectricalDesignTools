using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Raceways;
using EDTLibrary.ProjectSettings;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EDTLibrary.Managers;
public class DteqManager
{ 
    public static void SetDteqPd(DistributionEquipment dteq)
    {

        //dteq.PdType = EdtSettings.DteqDefaultPdTypeLV;
        dteq.PdSizeFrame = DataTableSearcher.GetBreakerFrame(dteq);
        dteq.PdSizeTrip = DataTableSearcher.GetBreakerTrip(dteq);

    }

    public static async Task<DistributionEquipment> AddDteq(object dteqToAddObject, ListManager listManager)
    {
        //Move AddDteq to DteqManager
        var _dteqFactory = new DteqFactory(listManager);
        DteqToAddValidator dteqToAddValidator = (DteqToAddValidator)dteqToAddObject;
        ErrorHelper.Log($"\n\n ******************* Add Dteq - Tag:{dteqToAddValidator.Tag}");

        try {
            var IsValid = dteqToAddValidator.IsValid(); //to help debug
            var errors = dteqToAddValidator._errorDict; //to help debug

            if (IsValid == false) return null;

            IDteq newDteq = _dteqFactory.CreateDteq(dteqToAddValidator);

            //Get Id manually
            //if (ListManager.IDteqList.Count == 0) {
            //    newDteq.Id = 1;
            //}
            //else {
            //    newDteq.Id = ListManager.IDteqList.Max(l => l.Id) + 1;
            //}

            IDteq dteqSubscriber = listManager.DteqList.FirstOrDefault(d => d == newDteq.FedFrom);
            if (dteqSubscriber != null) {
                //dteqSubscriber.AssignedLoads.Add(newDteq); load gets added to AssignedLoads inside DistributionManager.UpdateFedFrom();
                newDteq.LoadingCalculated += dteqSubscriber.OnAssignedLoadReCalculated;
                newDteq.PropertyUpdated += DaManager.OnDteqPropertyUpdated;
            }

            //Save to Db when calculating inside DteqModel
            newDteq.LoadCableDerating = double.Parse(EdtSettings.DteqLoadCableDerating);
            newDteq.CalculateLoading(); //after dteq is inserted to get a new Id
            listManager.AddDteq(newDteq);

            //Cable
            newDteq.CreatePowerCable();
            newDteq.SizePowerCable();
            newDteq.CalculateCableAmps();
            newDteq.PowerCable.SetTypeProperties();

            newDteq.PowerCable.Id = DaManager.prjDb.InsertRecordGetId(newDteq.PowerCable, GlobalConfig.CableTable, NoSaveLists.PowerCableNoSaveList);
            newDteq.PowerCable.PropertyUpdated += DaManager.OnPowerCablePropertyUpdated;
            listManager.CableList.Add(newDteq.PowerCable); // newCable is already getting added

            var racewayToAddValidator = new RacewayToAddValidator(listManager);
            racewayToAddValidator.Tag = newDteq.Tag + "-T01";
            racewayToAddValidator.Type = "LadderTray";
            racewayToAddValidator.Width = "24";
            racewayToAddValidator.Height = "6";

            RacewayModel newRaceway = await RacewayManager.AddRaceway(racewayToAddValidator, listManager);
            RacewayManager.AddRacewayRouteSegment(newRaceway, newDteq.PowerCable, listManager);

            return (DistributionEquipment)newDteq;
            
        }
        catch (Exception ex) {
            EdtNotificationService.SendError(dteqToAddValidator.Tag, ex.Message, "Add Dteq Error", ex);
            return null;
        }
    }

    public static async Task<int> DeleteDteqAsync(IDteq dteq, ListManager listManager)
    {
        try {

            IDteq dteqToDelete = DteqFactory.Recast(dteq);
            if (dteqToDelete != null) {
                //children first

                dteqToDelete.Tag = GlobalConfig.Deleted;
                listManager.UnregisterDteqFromLoadEvents(dteqToDelete);

                //to prevent the cable from being resaved to the database
                dteqToDelete.PowerCable.PropertyUpdated -= DaManager.OnPowerCablePropertyUpdated;


                DistributionManager.RetagLoadsOfDeleted(dteqToDelete);

                if (dteqToDelete.FedFrom != null) {
                    dteqToDelete.FedFrom.AssignedLoads.Remove(dteqToDelete);
                }
                listManager.DeleteDteq(dteqToDelete);
                DaManager.DeleteDteq(dteqToDelete);

                await Task.Delay(1000);
                await CableManager.DeletePowerCableAsync(dteqToDelete, listManager);


                dteqToDelete.Delete();
            }
        }
        catch (Exception ex) {
            EdtNotificationService.SendError(dteq.Tag, ex.Message, "Delete Dteq Error", ex);
        }
        return dteq.Id;
    }
}
