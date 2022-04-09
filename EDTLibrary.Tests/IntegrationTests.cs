using EDTLibrary.DataAccess;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;
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
            Assert.True(listManager.AreaList.Count == TestData.TestAreasList.Count);

            //Dteq
            //TestData.CreateTestDteqList();
            foreach (var dteq in TestData.TestDteqList) {
                dteq.Area = listManager.AreaList[0];
                dteqToAdd = new DteqToAddValidator(listManager, dteq);
                eqVm.AddDteq(dteqToAdd);
            }
            Assert.True(listManager.IDteqList.Count > 0); //causes an assert exception
            Assert.True(listManager.IDteqList.Count == TestData.TestDteqList.Count);

            //Loads
            foreach (var load in TestData.TestLoadList) {
                load.Area = listManager.AreaList[0];
                loadToAdd = new LoadToAddValidator(listManager, load);
                eqVm.AddLoad(loadToAdd);
                load.CalculateLoading();
            }
            Assert.True(listManager.LoadList.Count > 0); 
            Assert.True(listManager.LoadList.Count == TestData.TestLoadList.Count);

            //Cables
            int cableCount = listManager.IDteqList.Count + listManager.LoadList.Count;
            Assert.True(listManager.CableList.Count == cableCount);
            SelectAllDteqAndLoads(listManager, eqVm);

            IDteq dteqToCheck = listManager.IDteqList[2];
            Assert.True(dteqToCheck.DemandKva > 0);
            Assert.True(listManager.LoadList[0].DemandKva > 0);
            
            
            #endregion


            #region Rename, delete and re-add equipment
            //Rename
            listManager.IDteqList[0].Tag = "XTR-01";
            Assert.True(listManager.IDteqList[0].PowerCable.Destination == listManager.IDteqList[0].Tag);

            listManager.IDteqList[0].Tag = "TX-01";
            Assert.True(listManager.DteqList[1].PowerCable.Destination == listManager.DteqList[1].Tag);


            //LargestMotorTest
            //var xfrToTest = listManager.XfrList[2];
            //xfrToTest.FindLargestMotor(xfrToTest, new LoadModel { ConnectedKva = 0 });
            //Assert.True(xfrToTest.LargestMotor.Tag == "MTR-03");

            ////Delete Dteq
            //eqVm.DeleteDteq(listManager.IDteqList[0]);
            //Assert.True(listManager.IDteqList.Count == TestData.TestDteqList.Count - 1);
            //Assert.True(listManager.CableList.Count == cableCount - 1);

            #endregion


            //TODO - Clean up DteqModel vs abstract Dteq
            var list = DaManager.prjDb.GetRecords<DteqModel>(GlobalConfig.DteqTable);
            foreach (var item in list) {
                listManager.DteqList.Add(item);
            }
            Assert.True(listManager.DteqList.Count > 0);

            SelectAllDteqAndLoads(listManager, eqVm);
        }

        private static void SelectAllDteqAndLoads(ListManager listManager, EquipmentViewModel eqVm)
        {
            foreach (var dteq in listManager.IDteqList) {
                Debug.WriteLine(dteq.Tag.ToString());
                eqVm.SelectedDteq = dteq;
                foreach (var assignedLoad in dteq.AssignedLoads) {
                    Debug.WriteLine(assignedLoad.Tag.ToString());
                    eqVm.SelectedLoad = assignedLoad;
                }
            }
        }

        private static void DeleteAllRecords()
        {
            //Delete records
            DaManager.prjDb.DeleteAllRecords(GlobalConfig.AreaTable);

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