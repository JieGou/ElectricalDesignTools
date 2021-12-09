using EDTLibrary;
using EDTLibrary.DataAccess;
using EDTLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinFormCoreUI;

namespace WpfUI.ViewModels {

    public class EqViewModel : BaseViewModel{

        //Public Properties
        public string Test { get; set; }

        
        public ObservableCollection<DteqModel> DteqList { get; set; }
        public ObservableCollection<LoadModel> LoadList { get; set; }
        public ObservableCollection<CableModel> CableList { get; set; }


        public EqViewModel() {
            DteqList = new ObservableCollection<DteqModel>(ListManager.GetDteq());
            int i = 0;
            foreach (var item in DteqList) {
                i += 1;
                item.AssignedLoads.Add(new LoadModel() { Tag="Load"+i.ToString()});
            }
            LoadList = new ObservableCollection<LoadModel>(ListManager.GetLoads());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        //protected void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    int i = 0;
        //    foreach (var item in DteqList) {
        //        i += 1;
        //        item.AssignedLoads.Add(new LoadModel() { Tag = "Load" + i.ToString() });
        //    }
        //}


        public void AddDteq(DteqModel dteq) {
            DteqList.Add(dteq);
            int i = 0;
            foreach (var item in DteqList) {
                i += 1;
                item.AssignedLoads.Add(new LoadModel() { Tag = "Load" + i.ToString() });
            }

        }
    }
}
