using EDTLibrary.DataAccess;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Services;
using WpfUI.ViewModels;
using Xunit;

namespace EDTLibrary.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void IntegrationTest()
        {
            CopyDb();
            //Assert.True(File.Exists(GlobalConfig.TestDb));

            DaManager.prjDb = new SQLiteConnector(GlobalConfig.TestDb);
            DeleteAllRecords();


            #region INITIALIZATIONS

            //ListManager
            ListManager listManager = new ListManager();
            IDteq selectedDteq = new DteqModel();
            LoadModel selectedLoad = new LoadModel();

            StartupService startupService = new StartupService(listManager);
            startupService.InitializeLibrary();
            startupService.InitializeProject(GlobalConfig.TestDb);

            //Validators
            AreaToAddValidator areaToAdd;
            DteqToAddValidator dteqToAdd;
            LoadToAddValidator loadToAdd;

            //ViewModels
            AreasViewModel areaVm = new AreasViewModel(listManager);
            EquipmentViewModel eqVm = new EquipmentViewModel(listManager);

            #endregion

            #region Add Objects
            //Area
            foreach (var area in TestData.TestAreasList) {
                areaToAdd = new AreaToAddValidator(listManager, area);
                areaVm.AddArea(areaToAdd);
            }

            //Dteq
            foreach (var dteq in TestData.TestDteqList) {
                dteqToAdd = new DteqToAddValidator(listManager, dteq);
                eqVm.AddDteq(dteqToAdd);
            }
            Assert.True(listManager.DteqList.Count - 1 == TestData.TestDteqList.Count);


            //Loads
            foreach (var load in TestData.TestLoadList) {
                loadToAdd = new LoadToAddValidator(listManager, load);
                eqVm.AddLoad(loadToAdd);
                load.CalculateLoading();
            }

            foreach (var load in listManager.LoadList) {
                Debug.WriteLine(load.Tag.ToString());
            }

            Assert.True(listManager.AreaList.Count > 0);
            Assert.True(listManager.LoadList.Count > 0);
            Assert.True(listManager.LoadList.Count != TestData.TestLoadList.Count);
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




            //LargestMotorTest
            var xfrToTest = listManager.XfrList[0];
            xfrToTest.FindLargestMotor(xfrToTest, new LoadModel { ConnectedKva = 0 });
            Assert.True(xfrToTest.LargestMotor.Tag == "MTR-01b");

            //Delete
            eqVm.DeleteDteq(listManager.DteqList[1]);
            var listCount = (dteqCountOld - 1 - listManager.XfrList.Count - listManager.SwgList.Count - listManager.MccList.Count);

            Assert.True(listManager.DteqList.Count == dteqCountOld - 1);
            Assert.True(listManager.CableList.Count == cableCountOld - 1);

            #endregion



            //Todo - Clean up DteqModel vs abstract Dteq
            var list = DaManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable);
            foreach (var item in list) {
                listManager.DteqList.Add(item);
            }
            Assert.True(listManager.DteqList.Count > 0);
        }

        private static void DeleteAllRecords()
        {
            

            //Delete records
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.DteqTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.XfrTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.SwgTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.MccTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.LoadTable);
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.PowerCableTable);
        }

        [Fact]
        public void CopyDb()
        {
            File.Copy(GlobalConfig.DevDb, GlobalConfig.TestDb,true);
            Assert.True(File.Exists(GlobalConfig.TestDb));

        }
    }
}