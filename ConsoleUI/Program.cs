using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EDTLibrary;
using EDTLibrary.Models;
using LM = EDTLibrary.ListManager;

namespace ConsoleUI {
    class Program {



        static void Main(string[] args) {
            //TestClass tc = new TestClass();
            //tc.Name = "asdf";

            List<TestClass> list1 = new List<TestClass>();
            List<TestClass2> list2 = new List<TestClass2>();
            List<ITestClass> list3 = new List<ITestClass>();

            StaticClass.StaticList.Add(new TestClass { Name = "666" });

            list1.Add(StaticClass.StaticList[0]);
            




            //Console.WriteLine(tc.Name);
            Console.WriteLine(list1[0].Name);
            Console.WriteLine(StaticClass.StaticList[0].Name);
            

            list1[0].Name = "asdf111";
            //Console.WriteLine(tc.Name);
            Console.WriteLine(list1[0].Name);
            Console.WriteLine(StaticClass.StaticList[0].Name);


            list1[0].Name = "11111";
            // Console.WriteLine(tc.Name);
            Console.WriteLine(list1[0].Name);
            Console.WriteLine(StaticClass.StaticList[0].Name);

            Console.ReadLine();
        }


    }

    public interface ITestClass {
        string Name { get; set; }
    }

    public class TestClass: ITestClass {
        public string Name { get; set; }
    }

    public class TestClass2: ITestClass {
        public string Name { get; set; }
    }

    public static class StaticClass {
        public static List<TestClass> StaticList = new List<TestClass>();
    }
}
