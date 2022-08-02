using EDTLibrary.A_Helpers;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeTables;
using EDTLibrary.Models.aMain;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.TestDataFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfUI.Services;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Areas_and_Systems;
using WpfUI.ViewModels.Electrical;
using Xunit;

namespace EDTLibrary.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void IntegrationTest()
        {

            try {
                GlobalConfig.Testing = true;
                int sleepTime = 100;
                CopyDb();
                //Assert.True(File.Exists(GlobalConfig.TestDb));

                DaManager.prjDb = new SQLiteConnector(GlobalConfig.TestDb);

                //Clear Database
                DaManager.prjDb.DeleteAllRecords(GlobalConfig.AreaTable);
                DaManager.DeleteAllEquipmentRecords();

                #region INITIALIZATIONS

                //ListManager
                DaManager daManager = new DaManager();
                ListManager listManager = new ListManager();
                ScenarioManager.ListManager = listManager;
                TypeManager typeManager = new TypeManager();
                EdtSettings edtSettings = new EdtSettings();

                StartupService startupService = new StartupService(listManager);
                startupService.InitializeLibrary();
                startupService.InitializeProject(GlobalConfig.TestDb);

                //Validators
                AreaToAddValidator areaToAdd;
                DteqToAddValidator dteqToAdd;
                LoadToAddValidator loadToAdd;

                //ViewModels
                AreasViewModel areaVm = new AreasViewModel(listManager);
                MjeqViewModel eqVm = new MjeqViewModel(listManager);

                #endregion

                #region Add Objects

                //Area
                ErrorHelper.LogNoSave("\n\n\n-----------------AREA----------------- \n\n\n");

                foreach (var loopArea in TestData.TestAreasList) {
                    areaToAdd = new AreaToAddValidator(listManager, loopArea);
                    areaVm.AddArea(areaToAdd);
                }
                //Assert.True(listManager.AreaList.Count == TestData.TestAreasList.Count);


                //Dteq
                ErrorHelper.LogNoSave("\n\n\n-----------------DTEQ----------------- \n\n\n");

                foreach (var loopDteq in TestData.TestDteqList) {
                    loopDteq.Area = listManager.AreaList[0];
                    dteqToAdd = new DteqToAddValidator(listManager, loopDteq);
                    eqVm.AddDteq(dteqToAdd);
                }
                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.IDteqList.Count == TestData.TestDteqList.Count);


                //Loads
                ErrorHelper.LogNoSave("\n\n\n-----------------LOAD----------------- \n\n\n");

                foreach (var loopLoad in TestData.TestLoadList) {
                    loopLoad.Area = listManager.AreaList[0];
                    loadToAdd = new LoadToAddValidator(listManager, loopLoad);
                    eqVm.AddLoad(loadToAdd);
                }
                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.LoadList.Count == TestData.TestLoadList.Count);

                //Cables
                listManager.GetProjectTablesAndAssigments();
                int cableCount = listManager.IDteqList.Count + listManager.LoadList.Count;
                //Assert.True(listManager.CableList.Count == cableCount);

                ErrorHelper.SaveLog();
                #endregion



                //Components
                ErrorHelper.LogNoSave("\n\n\n-----------------Components----------------- \n\n\n");

                ILoad load = listManager.LoadList[0];
                load.DriveBool = true;
                load.DisconnectBool = true;
                IComponent drive = listManager.CompList[0]; // drive
                IComponent disc = listManager.CompList[1]; // load

                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.CompList.Count == 2);


                //Dteq.Area for Drive.Area
                ErrorHelper.LogNoSave("\n\n\n-----------------Dteq.Area for Drive.Area----------------- \n\n\n");

                DistributionEquipment dteq = listManager.DteqList.FirstOrDefault(d => d.Tag == "MCC-01");
                dteq.Area = listManager.AreaList[1];
                listManager.GetProjectTablesAndAssigments();
                // Assert.True(listManager.CompList[0].Area == dteq.Area);


                //Load.Area for Dcn.Area
                ErrorHelper.LogNoSave("\n\n\n-----------------Load.Area for Dcn.Area----------------- \n\n\n");

                listManager.LoadList[0].Area = listManager.AreaList[1];
                disc = listManager.CompList[1];
                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.CompList[1].Area == listManager.AreaList[1]);

                Thread.Sleep(1000);

                Assert.True(listManager.AreaList.Count == TestData.TestAreasList.Count);
                Assert.True(listManager.IDteqList.Count == TestData.TestDteqList.Count);
                Assert.True(listManager.LoadList.Count == TestData.TestLoadList.Count);
                //Assert.True(listManager.CableList.Count == cableCount);
                Assert.True(listManager.CompList.Count == 2);

                //Assert.True(listManager.CompList[0].Area == listManager.AreaList[1]);
                Assert.True(listManager.CompList[1].Area == listManager.AreaList[1]);



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
                SelectAllDteqAndLoads(listManager, eqVm);

                GlobalConfig.Testing = false;
            }

            finally {
                ErrorHelper.LogNoSave("IntegrationTest_finally");
                ErrorHelper.SaveLog();
                GlobalConfig.Testing = false;
            }

        }

        private static void SelectAllDteqAndLoads(ListManager listManager, MjeqViewModel eqVm)
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

       
        [Fact]
        public void CopyDb()
        {
            File.Copy(GlobalConfig.DevDb, GlobalConfig.TestDb,true);
            Assert.True(File.Exists(GlobalConfig.TestDb));

        }
    }
}