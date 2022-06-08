using EDTLibrary;
using EDTLibrary.LibraryData.TypeTables;
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
        public void IsTagAvailable_BothCases(string _dteqToAddTag, bool expected)
        {
            //Arrange
            ObservableCollection<DteqModel> DteqList = new ObservableCollection<DteqModel>()
            {
                new DteqModel() { Tag = "MCC-01" },
                new DteqModel() { Tag = "SWG-01" }
            };
            ObservableCollection<LoadModel> LoadList = new ObservableCollection<LoadModel>()
            {
                new LoadModel("MTR-01"),
                new LoadModel("HTR-01")
            };
            ListManager listManager = new ListManager();
            TypeManager typeManager = new TypeManager();

            MjeqViewModel eqv = new MjeqViewModel(listManager);

            //Act
            bool actual = TagAndNameValidator.IsTagAvailable(_dteqToAddTag, listManager);

            //Assert
            Assert.Equal(expected, actual);
        }
    }


}