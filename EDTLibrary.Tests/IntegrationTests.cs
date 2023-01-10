using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Components;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.ProjectSettings;
using EDTLibrary.TestDataFolder;
using EDTLibrary.UndoSystem;
using Syncfusion.PMML;
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
using WpfUI.ViewModels.AreasAndSystems;
using WpfUI.ViewModels.Electrical;
using WpfUI.ViewModels.Home;
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
                CopyDb();

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

                StartupService startupService = new StartupService(listManager, new ObservableCollection<PreviousProject>());
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
                ErrorHelper.Log("\n\n-----------------AREA----------------- \n");

                foreach (var loopArea in TestData.TestAreasList) {
                    ErrorHelper.Log("--------Add Area");
                    areaToAdd = new AreaToAddValidator(listManager, loopArea);
                    areaVm.AddArea(areaToAdd);
                }
                Assert.True(listManager.AreaList.Count == TestData.TestAreasList.Count);


                //Dteq
                ErrorHelper.Log("\n\n-----------------DTEQ----------------- \n");

                foreach (var loopDteq in TestData.TestDteqList_Full) {
                    ErrorHelper.Log($"--------Adding dteq: {loopDteq.Tag}");

                    loopDteq.Area = listManager.AreaList[0];
                    dteqToAdd = new DteqToAddValidator(listManager, loopDteq);
                    eqVm.AddDteq(dteqToAdd);
                }
                Assert.True(listManager.IDteqList.Count == TestData.TestDteqList_Full.Count);
                //Thread.Sleep(2000);
                listManager.GetProjectTablesAndAssigments();
                Assert.True(listManager.IDteqList.Count == TestData.TestDteqList_Full.Count);


                //Loads
                ErrorHelper.Log("\n\n-----------------LOAD----------------- \n");

                foreach (var loopLoad in TestData.TestLoadList_Full) {
                    ErrorHelper.Log($"--------Adding load: {loopLoad.Tag}");

                    loopLoad.Area = listManager.AreaList[0];
                    loadToAdd = new LoadToAddValidator(listManager, loopLoad);
                    eqVm.AddLoad(loadToAdd);
                }

                Assert.True(listManager.LoadList.Count == TestData.TestLoadList_Full.Count);
                //Thread.Sleep(2000);
                listManager.GetProjectTablesAndAssigments();
                Assert.True(listManager.LoadList.Count == TestData.TestLoadList_Full.Count);

                //Cables
                Thread.Sleep(1000);
                listManager.GetProjectTablesAndAssigments();
                int cableCount = listManager.IDteqList.Count + listManager.LoadList.Count;
                Assert.True(listManager.CableList.Count == cableCount);

                ErrorHelper.SaveLog();
                #endregion



                //Components
                ErrorHelper.Log("\n\n-----------------Components----------------- \n");

                ILoad load = listManager.LoadList[0];
                load.StandAloneStarterBool = true;
                load.DisconnectBool = true;
                IComponentEdt drive = listManager.CompList[0]; // drive
                IComponentEdt disconnect = listManager.CompList[1]; // load

                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.CompList.Count == 2);


                //Dteq.Area for StandAloneStarter.Area
                ErrorHelper.Log("\n\n-----------------Dteq.Area for StandAloneStarter.Area----------------- \n");

                IDteq mcc01 = listManager.DteqList.FirstOrDefault(d => d.Tag == "MCC-01");
                mcc01.Area = listManager.AreaList[2];

                Thread.Sleep(1000);
                listManager.GetProjectTablesAndAssigments();
                // Assert.True(listManager.CompList[0].Area == dteq.Area);


                //Load.Area for Dcn.Area
                ErrorHelper.Log("\n\n-----------------Load.Area for Dcn.Area----------------- \n");

                listManager.LoadList[0].Area = listManager.AreaList[1];
                disconnect = listManager.CompList[1];

                Thread.Sleep(3000);
                listManager.GetProjectTablesAndAssigments();
                //Assert.True(listManager.CompList[1].Area == listManager.AreaList[1]);


                Assert.True(listManager.AreaList.Count == TestData.TestAreasList.Count);
                Assert.True(listManager.IDteqList.Count == TestData.TestDteqList_Full.Count);
                //Assert.True(listManager.LoadList.Count == TestData.TestLoadList.Count);
                //Assert.True(listManager.CableList.Count == cableCount);
                Assert.True(listManager.CompList.Count == 2);

                //Assert.True(listManager.CompList[0].Area == listManager.AreaList[1]);
                Assert.True(listManager.CompList[1].Area == listManager.AreaList[1]);
                Assert.True(listManager.CompList[0].Area == listManager.AreaList[2]);



                #region Rename, delete and re-add equipment
                //Rename
                listManager.IDteqList[0].Tag = "XTR-01";
                Assert.True(listManager.IDteqList[0].PowerCable.Destination == listManager.IDteqList[0].Tag);

                listManager.IDteqList[0].Tag = "TX-01";
                Assert.True(listManager.DteqList[1].PowerCable.Destination == listManager.DteqList[1].Tag);


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
                ErrorHelper.Log("IntegrationTest_finally");
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