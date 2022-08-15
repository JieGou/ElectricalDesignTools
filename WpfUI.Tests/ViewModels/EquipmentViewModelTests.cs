using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
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
            ObservableCollection<DistributionEquipment> dteqList = new ObservableCollection<DistributionEquipment>()
            {
                new DteqModel() { Tag = "MCC-01" },
                new DteqModel() { Tag = "SWG-01" }
            };
            ObservableCollection<ILoad> loadList = new ObservableCollection<ILoad>()
            {
                new LoadModel("MTR-01"),
                new LoadModel("HTR-01")
            };
            ListManager listManager = new ListManager();
            listManager.DteqList = dteqList;
            listManager.LoadList = loadList;
            //Act
            bool actual = TagAndNameValidator.IsTagAvailable(eqTagToAdd, listManager);

            //Assert
            Assert.Equal(expected, actual);
        }
    }


}