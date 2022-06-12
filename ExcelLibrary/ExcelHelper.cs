using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;


namespace ExcelLibrary
{
    public class ExcelHelper
    {
        private readonly bool _visible;
        private readonly bool _displayAlerts;
        private readonly bool _screenUpdating;

        public ExcelHelper(bool visible = true, bool displayAlerts = false, bool screenUpdating = true)
        {
            _visible = visible;
            _displayAlerts = displayAlerts;
            _screenUpdating = screenUpdating;
        }

        public Excel.Application App { get; set; }
        public Excel.Range Range { get; set; }
        public Excel.Workbooks Workbooks { get; set; }
        public Excel.Workbook Workbook { get; set; }
        public Excel.Worksheet Sheet { get; set; }


        public void Initialize(string sheetName = "Sheet1")
        {
            App = new Excel.Application();
            App.Visible = _visible;
            App.DisplayAlerts = _displayAlerts;
            App.ScreenUpdating = _screenUpdating;

            Workbooks = App.Workbooks; ;
            Workbook = Workbooks.Add();
            Sheet = Workbook.Sheets["Sheet1"];
            if (sheetName != "Sheet1") {
                Sheet.Name = sheetName;
            }
            
        }

        public async void ExportTest(string fileLocation = "C:\\temp\\", string filename = "Edt Excel Test")
        {
            try {

                for (int i = 0; i < 10; i++) {
                    int rowNum = i + 1;

                    for (int j = 0; j < 10; j++) {
                        int colNum = j + 1;

                        Sheet.Cells[rowNum, colNum] = $"{rowNum}, {colNum}";
                        if (rowNum == 1) {
                            Sheet.Cells[rowNum, colNum].Interior.Color = Excel.XlRgbColor.rgbPaleVioletRed;
                        }
                        if (rowNum == 2) {
                            Sheet.Cells[rowNum, colNum].Interior.Color = System.Drawing.Color.FromArgb(150, 254, 254);
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("Write Complete. " + DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex) {
                string errMsg = "Error Saving Excel File - " + ex.Message;
                System.Diagnostics.Debug.WriteLine(errMsg);
                MessageBox.Show(errMsg);

                Release();
                //or
                //CollectGarbage();
                //or
                //KillExcelAsync();
            }
        }

        public void ExportObjectProperties<T>(T classObject, List<string> propertiesToIgnore) where T : class, new()
        {
            var objectProperties = classObject.GetType().GetProperties();
            List<string> propsToWrite = new List<string>();
            int row = 1;
            int col = 1;
            //Property Names
            bool addProp;
            foreach (var prop in objectProperties) {
                addProp = true;
                foreach (var propToIgnore in propertiesToIgnore) {
                    if (prop.Name == propToIgnore) {
                        addProp = false;
                    }
                }
                if (addProp == true) {
                    propsToWrite.Add(prop.Name);
                }
                
            }

            foreach (var prop in propsToWrite) {
                Sheet.Cells[row, col] = $"{prop}";
                col += 1;
            }

        }


        public async Task SaveWorkbook(string fileLocation = "C:\\temp\\", string filename = "Edt Excel Test")
        {

            try {
                var filePath = fileLocation + filename + DateTime.Now.ToString().Replace(":", "_");
                Workbook.SaveAs(filePath,
                    Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value,
                    Missing.Value,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value,
                    Missing.Value);

                System.Diagnostics.Debug.WriteLine("Save Complete. " + DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex) {
                string errMsg = "Error Saving Excel File - " + ex.Message;
                System.Diagnostics.Debug.WriteLine(errMsg);
                MessageBox.Show(errMsg);
            }

            finally {

                Release();
                //or
                //CollectGarbage();
                //or
                //KillExcelAsync();

            }
        }

        public void Release()
        {

            //must release each object that referenced a COM object

            if (Sheet == null || 
                Workbook == null || 
                Workbooks == null || 
                App == null)
              return;
            
            System.Runtime.InteropServices.Marshal.ReleaseComObject(Sheet);
            Sheet = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(Workbook);
            Workbook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(Workbooks);
            Workbooks = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(App);
            App = null;
          
        }
        public async Task CollectGarbage()
        {
            //await Task.Delay(5000);
            //Process[] excelProcesses = Process.GetProcessesByName("excel");
            //foreach (Process p in excelProcesses) {
            //    if (string.IsNullOrEmpty(p.MainWindowTitle)) // use MainWindowTitle to distinguish this excel process with other excel processes 
            //    {
            //        p.Kill();
            //    }

            //}

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private static async Task KillExcelAsync()
        {
            await Task.Delay(5000);
            Process[] excelProcesses = Process.GetProcessesByName("excel");
            foreach (Process p in excelProcesses) {
                if (string.IsNullOrEmpty(p.MainWindowTitle)) // use MainWindowTitle to distinguish this excel process with other excel processes 
                {
                    p.Kill();
                }

                //}

            }
        }
    }
}
