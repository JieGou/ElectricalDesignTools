using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDTLibrary.Tests.Models.DistributionEquipment
{
    public class DteqToAddValidatorTest
    {
        [Theory]
        [InlineData("XFR-05", "XFR", "ML", GlobalConfig.Utility, "2000", "kVA", "480", "460")]
        [InlineData("MCC-06", "MCC", "FL", "SWG-01", "2000", "A", "480", "460" )]
        [InlineData("MCC-07", "MCC", "ML", "SWG-02", "2000", "A", "600", "600" )]
        public void IsValid_True(string tag, string type, string areaTag, string fedFrom, string size, string unit,string lineVoltage, string loadVoltage )
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager _listManager = new ListManager();
            _listManager.AreaList = TestData.TestAreasList;
            _listManager.DteqList = TestData.TestDteqList;
            _listManager.LoadList = TestData.TestLoadList;
            DteqModel selectedDteq = new DteqModel();

            //Act
            DteqToAddValidator DteqToAdd = new DteqToAddValidator(_listManager, selectedDteq);
            DteqToAdd.Tag = tag;
            DteqToAdd.Type = type;
            DteqToAdd.AreaTag = areaTag;
            DteqToAdd.FedFromTag = fedFrom;
            DteqToAdd.Size = size;
            DteqToAdd.Unit = unit;
            DteqToAdd.LineVoltage = lineVoltage;
            DteqToAdd.LoadVoltage = loadVoltage;


            bool actual = DteqToAdd.IsValid();
            var errors = DteqToAdd._errorDict;
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
            ListManager _listManager = new ListManager();
            _listManager.DteqList = TestData.TestDteqList;
            _listManager.LoadList = TestData.TestLoadList;
            DteqModel selectedDteq = new DteqModel();

            //Act
            DteqToAddValidator DteqToAdd = new DteqToAddValidator(_listManager, selectedDteq);
            DteqToAdd.Tag = tag;
            DteqToAdd.Type = type;
            DteqToAdd.AreaTag = areaTag;
            DteqToAdd.FedFromTag = fedFrom;
            DteqToAdd.Size = size;
            DteqToAdd.Unit = unit;
            DteqToAdd.LineVoltage = lineVoltage;
            DteqToAdd.LoadVoltage = loadVoltage;

            bool actual = DteqToAdd.IsValid();
            var errors = DteqToAdd._errorDict;
            GlobalConfig.Testing = false;

            Assert.False(actual);
        }
    }
}
