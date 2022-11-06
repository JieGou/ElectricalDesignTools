using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.TestDataFolder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUI.Services;
using WpfUI.ViewModels.Home;
using Xunit;

namespace EDTLibrary.Tests.Models.DistributionEquipment
{
    public class DteqToAddValidatorTest
    {
        [Theory]
        [InlineData("XFR-07", "XFR", "ML", GlobalConfig.Utility, "2000", "kVA", "480", "460")]
        [InlineData("MCC-08", "MCC", "ML", "SWG-03", "2000", "A", "480", "480" )]
        [InlineData("MCC-09", "MCC", "ML", "SWG-02", "2000", "A", "600", "600" )]
        public void IsValid_True(string tag, string type, string areaTag, string fedFrom, string size, string unit,string lineVoltage, string loadVoltage )
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager listManager = new ListManager();
            listManager.AreaList = TestData.TestAreasList;
            listManager.DteqList = TestData.TestDteqList;
            listManager.LoadList = TestData.TestLoadList;
            DteqModel dteqToAdd = new DteqModel();
            dteqToAdd.Area = TestData.TestAreasList[0];

            StartupService startupService = new StartupService(listManager, new ObservableCollection<PreviousProject>());
            startupService.InitializeLibrary();
            TypeManager.GetTypeTables();

            //Act
            DteqToAddValidator dteqToAddValidator = new DteqToAddValidator(listManager, dteqToAdd);
            dteqToAddValidator.Tag = tag;
            dteqToAddValidator.Type = type;
            dteqToAddValidator.AreaTag = areaTag;
            dteqToAddValidator.FedFromTag = fedFrom;
            dteqToAddValidator.Size = size;
            dteqToAddValidator.Unit = unit;
            dteqToAddValidator.LineVoltage = lineVoltage;
            dteqToAddValidator.LoadVoltage = loadVoltage;
            dteqToAddValidator.LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(lineVoltage));
            dteqToAddValidator.LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(loadVoltage));

            GlobalConfig.SelectingNew = false;
            bool actual = dteqToAddValidator.IsValid();
            var errors = dteqToAddValidator._errorDict;
            GlobalConfig.Testing = false;

            Assert.True( actual );
        }

        [Theory]
        [InlineData("", "", "ML", "", "", "", "", "")] //all
        [InlineData("", "MCC", "ML", "SWG-01", "2000", "A", "600", "600")] //tag
        [InlineData("MCC-07", "", "ML", "SWG-01", "2000", "A", "600", "600")] //type
        [InlineData("XFR-10", "XFR", "bad area", "UTILITY", "2000", "A", "110000", "")] //area
        [InlineData("MCC-08", "MCC", "ML", "", "", "A", "600", "600")] //Supplier or FedFrom
        [InlineData("MCC-09", "MCC", "ML", "Empty Dteq", "2000", "A", "600", "")] //FedFrom and/or load voltage
        [InlineData("XFR-10", "XFR", "ML", "UTILITY", "2000", "A", "110000", "")] //unit
        [InlineData(GlobalConfig.EmptyTag, "MCC", "SWG-02", "ML", "2000", "A", "480", "460")] //line voltage mismatch

        public void IsValid_False(string tag, string type, string areaTag, string fedFrom, string size, string unit, string lineVoltage, string loadVoltage)
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager listManager = new ListManager();
           
            DteqModel dteqToAdd = new DteqModel();
            dteqToAdd.Area = TestData.TestAreasList[0];
            dteqToAdd.LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(lineVoltage));
            dteqToAdd.LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(loadVoltage));

            StartupService startupService = new StartupService(listManager, new ObservableCollection<PreviousProject>());
            startupService.InitializeLibrary();
            TypeManager.GetTypeTables();
            listManager.DteqList = TestData.TestDteqList;
            listManager.LoadList = TestData.TestLoadList;

            //Act
            DteqToAddValidator dteqToAddValidator = new DteqToAddValidator(listManager, dteqToAdd);
            dteqToAddValidator.Tag = tag;
            dteqToAddValidator.Type = type;
            dteqToAddValidator.AreaTag = areaTag;
            dteqToAddValidator.FedFromTag = fedFrom;
            dteqToAddValidator.Size = size;
            dteqToAddValidator.Unit = unit;
            dteqToAddValidator.LineVoltage = lineVoltage;
            dteqToAddValidator.LoadVoltage = loadVoltage;
            dteqToAddValidator.LineVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(lineVoltage));
            dteqToAddValidator.LoadVoltageType = TypeManager.VoltageTypes.FirstOrDefault(vt => vt.Voltage == Double.Parse(loadVoltage));

            bool actual = dteqToAddValidator.IsValid();
            var errors = dteqToAddValidator._errorDict;
            GlobalConfig.Testing = false;

            Assert.False(actual);
        }
    }
}
