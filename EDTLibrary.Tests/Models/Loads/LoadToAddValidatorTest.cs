using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.TestDataFolder;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDTLibrary.Tests.Models.Loads
{
    public class LoadToAddValidatorTest
    {
        [Theory]
        [InlineData("MTR-13", "MOTOR", "ML", "MCC-02", "50", "HP", "575")] 
        public void IsValid_True(string tag, string type, string area, string fedFrom, string size, string unit, string voltage){
            GlobalConfig.Testing = true;

            //Arrange
            ListManager _listManager = new ListManager();
            _listManager.DteqList = TestData.TestDteqList;
            _listManager.LoadList = TestData.TestLoadList;
            _listManager.AreaList = TestData.TestAreasList;

            LoadModel loadToAdd = new LoadModel { Area = TestData.TestAreasList[0]};

            //Act
            LoadToAddValidator loadToAddValidator = new LoadToAddValidator(_listManager, loadToAdd);
            loadToAddValidator.Tag = tag;
            loadToAddValidator.Type = type;
            loadToAddValidator.AreaTag = area;
            loadToAddValidator.FedFromTag = fedFrom;
            loadToAddValidator.Size = size;
            loadToAddValidator.Unit = unit;
            loadToAddValidator.Voltage = voltage;


            bool actual = loadToAddValidator.IsValid();
            var errors = loadToAddValidator._errorDict;
            GlobalConfig.Testing = false;

            Assert.True(actual);
        }

        [Theory]
        [InlineData("HTR-10", "", "MCC-01", "2000", "kVA", "480")] //no type
        [InlineData("MTR-11", "MOTOR", GlobalConfig.Utility, "2000", "HP", "480")] // fed from utility
        [InlineData("MTR-12", "MOTOR", "MCC-02", "2000", "A", "600")] //wrong units
        [InlineData("MTR-13", "MOTOR", "MCC-X", "50", "HP", "600")] // Dteq Doesn't exist
        public void IsValid_False(string tag, string type, string fedFrom, string size, string unit, string voltage)
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager _listManager = new ListManager();
            _listManager.DteqList = TestData.TestDteqList;
            _listManager.LoadList = TestData.TestLoadList;
            DteqModel selectedDteq = new DteqModel();

            //Act
            LoadToAddValidator LoadToAdd = new LoadToAddValidator(_listManager);
            LoadToAdd.Tag = tag;
            LoadToAdd.Type = type;
            LoadToAdd.FedFromTag = fedFrom;
            LoadToAdd.Size = size;
            LoadToAdd.Unit = unit;
            LoadToAdd.Voltage = voltage;

            GlobalConfig.SelectingNew = false;
            bool actual = LoadToAdd.IsValid();
            var errors = LoadToAdd._errorDict;
            GlobalConfig.Testing = false;

            Assert.False(actual);
        }
    }
}
