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
using EDTLibrary.Models.Loads;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Scenario sc1 = new Scenario();
            Scenario sc2 = new Scenario();
            Scenario sc3 = new Scenario();

            sc1.CreateSampleData();
            sc2 = sc1;


            sc1.loadList[0].Tag = "easdfa";

            sc1 = sc3;

            Console.WriteLine("Scenario 1:");

            foreach (var load in sc1.loadList) {
                Console.WriteLine(load.Tag);
            }

            Console.WriteLine("Scenario 2:");
            foreach (var load in sc2.loadList) {
                Console.WriteLine(load.Tag);
            }
            Console.ReadLine();
        }
    }

    public interface ICloneable<T>
    {
        T Clone();
    }
    public class Scenario : ICloneable<Scenario>
    {
        public List<LoadModel> loadList = new List<LoadModel>();

        public void CreateSampleData()
        {
            loadList.Clear();
            loadList.AddRange(
                new LoadModel[] {
                    new LoadModel { Tag = "PP-01" },
                    new LoadModel { Tag = "PP-01" }
                });
        }
        public Scenario Clone()
        {
            return new Scenario { loadList = this.loadList };
        }
    }
}