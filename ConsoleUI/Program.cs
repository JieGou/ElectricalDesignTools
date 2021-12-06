using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EDTLibrary;
using EDTLibrary.Models;

namespace ConsoleUI {
    class Program {



        static void Main(string[] args) {


            ListManager.loadListOC.CollectionChanged += ListManager.TagChanged_Event;
            ListManager.loadListOC.Add(new LoadModel { Tag = "test" });
            ListManager.loadListOC[0].Tag = "test2";
            ListManager.loadListOC.Add(new LoadModel { Tag = "1" });
            Console.ReadLine(); 
        }

       
    }

    public class TheClass : INotifyPropertyChanged {

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected void SetPropertyField<T>(string propertyName, ref T field, T newValue) {
            if (!EqualityComparer<T>.Default.Equals(field, newValue)) {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _property1;
        public int Property1 {
            get { return _property1; }
            set { SetPropertyField("Property1", ref _property1, value); }
        }

        private string _property2;
        public string Property2 {
            get { return _property2; }
            set { SetPropertyField("Property2", ref _property2, value); }
        }

        private double _property3;
        public double Property3 {
            get { return _property3; }
            set { SetPropertyField("Property3", ref _property3, value); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

}