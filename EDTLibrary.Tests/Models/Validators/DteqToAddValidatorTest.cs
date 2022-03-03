using EDTLibrary.Models.DistributionEquipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EDTLibrary.Tests.Models.Validators
{
    public class DteqToAddValidatorTest
    {
        [Theory]
        [InlineData("XFR-05", "XFR", GlobalConfig.Utility, "2000", "kVA", "480", "460")]
        [InlineData("MCC-06", "MCC", "SWG-01", "2000", "A", "480", "460" )]
        [InlineData("MCC-07", "MCC", "SWG-02", "2000", "A", "600", "600" )]
        public void IsValid_True(string tag, string type, string fedFrom, string size, string unit,string lineVoltage, string loadVoltage )
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager _listManager = new ListManager();
            _listManager.DteqList = GlobalConfig.TestDteqList;
            _listManager.LoadList = GlobalConfig.TestLoadList;
            DteqModel selectedDteq = new DteqModel();

            //Act
            DteqToAddValidator DteqToAdd = new DteqToAddValidator(_listManager, selectedDteq);
            DteqToAdd.Tag = tag;
            DteqToAdd.Type = type;
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
        [InlineData("", "", "", "", "", "", "")]
        [InlineData("MCC-01", "", "", "", "", "", "")]
        [InlineData("", "MCC", "SWG-01", "2000", "A", "600", "600")]
        [InlineData("MCC-07", "", "SWG-01", "2000", "A", "600", "600")]
        [InlineData("MCC-07", "MCC", "", "", "A", "600", "600")]
        [InlineData("MCC-07", "MCC", "SWG-01", "2000", "", "600", "600")]
        [InlineData("MCC-07", "MCC", "SWG-01", "2000", "A", "", "600")]
        [InlineData("MCC-07", "MCC", "SWG-01", "2000", "A", "600", "")]

        [InlineData("XFR-05", "XFR", GlobalConfig.Utility, "2000", "A", "480", "460")]
        [InlineData(GlobalConfig.EmptyTag, "MCC", "SWG-02", "2000", "A", "480", "460")]
        [InlineData("MCC-06", "MCC", "SWG-02", "2000", "A", "480", "460")]
        [InlineData("MCC-07", "MCC", "SWG-01", "2000", "A", "600", "600")]
        [InlineData("MCC-01", "MCC", "SWG-01", "2000", "A", "600", "600")]
        public void IsValid_False(string tag, string type, string fedFrom, string size, string unit, string lineVoltage, string loadVoltage)
        {
            GlobalConfig.Testing = true;

            //Arrange
            ListManager _listManager = new ListManager();
            _listManager.DteqList = GlobalConfig.TestDteqList;
            _listManager.LoadList = GlobalConfig.TestLoadList;
            DteqModel selectedDteq = new DteqModel();

            //Act
            DteqToAddValidator DteqToAdd = new DteqToAddValidator(_listManager, selectedDteq);
            DteqToAdd.Tag = tag;
            DteqToAdd.Type = type;
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
