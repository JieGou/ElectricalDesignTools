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
            CreateSampleData(); //
            Stopwatch timer = new Stopwatch();

            timer.Start();

            LM.CalculateLoads(LM.loadList);
            LM.AssignLoadsToDteq(LM.dteqList, LM.loadList);

            LM.masterLoadList = LM.CreateMasterLoadList(LM.dteqList, LM.loadList);
            LM.compList = LM.CreateComponentList(LM.loadList);
            LM.cableList = LM.CreateCableList(LM.masterLoadList);

            //print iLoads
            foreach (var iload in LM.masterLoadList) {
                Console.WriteLine(GetProperties(iload, "Tag"));
            }

            //Print Load Components
            foreach (var load in LM.loadList) {
                Console.WriteLine(load.Tag);
                foreach (var component in load.InLineComponents) {
                    Console.WriteLine(" - " + component.Tag);
                }
            }

            //Print Component List
            foreach (var component in LM.compList) {
                Console.WriteLine(component.ComponentOf + ": " + component.Tag);
            }

            //Print Cables
            Console.WriteLine();
            foreach (var cable in LM.cableList) {
                //Console.WriteLine(cable.From.Replace("-","") + GlobalConfig.Separator + cable.To.Replace("-", ""));
                cable.CreateCableTag();
                Console.WriteLine(cable.Tag);
            }

            string test = Categories.CABLE.ToString();
            Console.WriteLine("enum category test: " + test);

            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:0000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.WriteLine();
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("\n");
            Console.WriteLine("Checking for duplicate tag");
            LM.CreateEqDict();

            Console.WriteLine(LM.CheckDuplicateTags("MCC-01").ToString());
            Console.ReadLine();
        }


        public static string GetProperties(Object obj, string propName = "") {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();

            if (propName != "") {
                sb.AppendLine($"{obj.GetType().GetProperty(propName).GetValue(obj).ToString()}");
            }

            foreach (PropertyInfo prop in properties) {
                sb.AppendLine($" - {prop.Name}: {prop.GetValue(obj)}  ");
            }

            return sb.AppendLine().ToString();
        }

        public static void CreateSampleData() {

            //List<MajorEquipmentModel> dteqList= new();
            LM.dteqList.Add(new DistributionEquipmentModel { Id = 2, Tag = "XFR-01", Type = "XFR", LineVoltage = 4160, LoadVoltage = 480, Unit = "kW", FedFrom = "UTILITY" });
            LM.dteqList.Add(new DistributionEquipmentModel { Id = 2, Tag = "SWG-01", Type = "SWG", LineVoltage = 480, LoadVoltage = 480, Unit = "kW", FedFrom = "XFR-01" });
            LM.dteqList.Add(new DistributionEquipmentModel { Id = 3, Tag = "MCC-01", Type = "MCC", LineVoltage = 480, LoadVoltage = 460, Unit = "kW", FedFrom = "SWG-01" });

            //List<LoadModel> loadList = new();
            LM.loadList.Add(new LoadModel { Id = 1, Tag = "PP-01", Type = "MOTOR", Voltage = 460, Size = 50, Unit = "HP", FedFrom = "MCC-01", LoadFactor = 1, Efficiency = 85, PowerFactor = 85 });
            LM.loadList.Add(new LoadModel { Id = 1, Tag = "HTR-01", Type = "HEATER", Voltage = 480, Size = 50, Unit = "HP", FedFrom = "MCC-01", LoadFactor = 1 });
            LM.loadList.Add(new LoadModel { Id = 1, Tag = "LTX-01", Type = "TRANSFORMER", Voltage = 480, Size = 50, Unit = "HP", FedFrom = "MCC-01", LoadFactor = 1 });

            //Add component to load PP-01
            //need to add components to master list first to check for duplicate tags
            LM.loadList[0].InLineComponents.Add(new ComponentModel { Tag = "DCN-01", ComponentOf = "PP-01" });
            LM.loadList[0].InLineComponents.Add(new ComponentModel { Tag = "VFD-01", ComponentOf = "PP-01" });
            LM.loadList[0].InLineComponents.Add(new ComponentModel { Tag = "DCN-02", ComponentOf = "PP-01" });

            LM.loadList[1].InLineComponents.Add(new ComponentModel { Tag = "DCN-03", ComponentOf = "PP-01" });

        }
    }
}
