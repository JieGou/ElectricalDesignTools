using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Calculations;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using System.Collections.ObjectModel;
using WpfUI.ViewModels;
using WpfUI.ViewModels.Electrical;
using Xunit;

namespace WpfUI.Tests.ViewModels
{
    public class EquipmentViewModelTests
    {

        [Theory]
        [InlineData("MCC-01", false)]
        [InlineData("PP-01", true)]
        public void IsTagAvailable_BothCases(string eqTagToAdd, bool expected)
        {
            //Arrange
            ObservableCollection<IDteq> dteqList = new ObservableCollection<IDteq>()
            {
                new DteqModel() { Tag = "MCC-01" },
                new DteqModel() { Tag = "SWG-01" }
            };
            ObservableCollection<ILoad> loadList = new ObservableCollection<ILoad>()
            {
                new LoadModel(){Tag="MTR-01"},
                new LoadModel(){Tag="HTR-01"}
            };
            ListManager listManager = new ListManager();
            listManager.IDteqList = dteqList;
            listManager.LoadList = loadList;
            //Act
            DaManager.GettingRecords = false;
            bool actual = TagAndNameValidator.IsTagAvailable(eqTagToAdd, listManager);

            //Assert
            Assert.Equal(expected, actual);
        }
    }


}