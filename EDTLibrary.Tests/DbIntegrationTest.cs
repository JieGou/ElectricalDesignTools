using EDTLibrary.DataAccess;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Services;
using WpfUI.ViewModels;
using Xunit;

namespace EDTLibrary.Tests
{
    public class DbIntegrationTest
    {
        [Fact]
        public void CopyDb_Test()
        {
            CopyDb();
            //Assert.True(File.Exists(GlobalConfig.TestDb));

            DbManager.prjDb = new SQLiteConnector(GlobalConfig.TestDb);
            
            //Delete records
            DbManager.prjDb.DeleteAllRecords(GlobalConfig.DteqTable);   
            DbManager.prjDb.DeleteAllRecords(GlobalConfig.LoadTable);
            DbManager.prjDb.DeleteAllRecords(GlobalConfig.PowerCableTable);


            #region INITIALIZATIONS

            //ListManager
            ListManager listManager = new ListManager();
            IDteq selectedDteq = new DteqModel();
            LoadModel selectedLoad = new LoadModel();

            StartupService startupService = new StartupService(listManager);
            startupService.InitializeLibrary();
            startupService.InitializeProject(GlobalConfig.TestDb);

            //Validators
            DteqToAddValidator dteqToAdd;
            LoadToAddValidator loadToAdd = new LoadToAddValidator(listManager);

            //ViewModels
            EquipmentViewModel eqView = new EquipmentViewModel(listManager);

            #endregion

            #region Create Objects
            foreach (var dteq in TestData.TestDteqList) {
                dteqToAdd = new DteqToAddValidator(listManager, dteq);
                eqView.AddDteq(dteqToAdd);
            }
            Assert.True(listManager.DteqList.Count > 0);

            foreach (var load in TestData.TestLoadList) {
                loadToAdd = new LoadToAddValidator(listManager, load);
                eqView.AddLoad(loadToAdd);
                load.CalculateLoading();
            }
            Assert.True(listManager.LoadList.Count > 0);
            Assert.True(listManager.DteqList[0].DemandKva > 0);
            Assert.True(listManager.LoadList[0].DemandKva > 0);
            #endregion


            #region Rename, delete and re-add equipment

            //Rename
            listManager.IDteqList[0].Tag = "XTR-01";
            Assert.True(listManager.IDteqList[0].PowerCable.Tag.Contains("XTR01"));

            listManager.DteqList[1].Tag = "TX-01";
            Assert.True(listManager.DteqList[1].PowerCable.Tag.Contains("TX01"));

            var dteqCountOld = listManager.DteqList.Count;
            var loadCountOld = listManager.LoadList.Count;
            var cableCountOld = listManager.CableList.Count;

            //Delete
            eqView.DeleteDteq(listManager.DteqList[1]);
            Assert.True(listManager.DteqList.Count == dteqCountOld-1);
            //Assert.True(listManager.DteqList.Count == listManager.DteqList.Count-1);
            Assert.True(listManager.CableList.Count == cableCountOld-1);
            //Assert.True(listManager.CableList.Count == listManager.CableList.Count-1);

            #endregion


            listManager.DteqList = DbManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable);
            Assert.True(listManager.DteqList.Count > 0);
        }

        [Fact]
        public void CopyDb()
        {
            File.Copy(GlobalConfig.DevDb, GlobalConfig.TestDb,true);
            Assert.True(File.Exists(GlobalConfig.TestDb));

        }
    }
}