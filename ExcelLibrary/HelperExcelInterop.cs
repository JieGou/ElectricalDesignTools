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
    public class HelperExcelInterop
    {
        public static async void WriteToExcel(string fileLocation = "C:\\temp\\", string filename = "Edt Excel Test")
        {
            object oMissing = Missing.Value;

            Excel.Application xlApp = null;
            Excel.Range xlRange = null;
            Excel.Workbooks xlBooks = null;
            Excel.Workbook xlBook = null;
            Excel.Worksheet xlSheet = null;

            int worksheetCount = 0;

            try {
                //Initialize

                xlApp = new Excel.Application();
                xlApp.DisplayAlerts = false;
                xlApp.Visible = true;
                //excelApp.UserControl = false;
                //excelApp.Calculation = Excel.XlCalculation.xlCalculationManual;
                //excelApp.ScreenUpdating = false;

                //Create 
                xlBooks = xlApp.Workbooks;
                xlBook = xlBooks.Add();
                worksheetCount = xlBook.Sheets.Count;
                xlSheet = xlBook.Sheets["Sheet1"];

                for (int i = 0; i < 10; i++) {
                    int rowNum = i + 1;

                    for (int j = 0; j < 10; j++) {
                        int colNum = j + 1;

                        xlSheet.Cells[rowNum, colNum] = $"{rowNum}, {colNum}";
                        if (rowNum == 1) {
                            xlSheet.Cells[rowNum, colNum].Interior.Color = Excel.XlRgbColor.rgbPaleVioletRed;
                        }
                        if (rowNum == 2) {
                            xlSheet.Cells[rowNum, colNum].Interior.Color = System.Drawing.Color.FromArgb(150, 254, 254);
                        }
                        
                    }
                }

                //enable
                //excelApp.Calculation = Excel.XlCalculation.xlCalculationManual;
                //excelApp.ScreenUpdating = true;

                //save Workbook - if file exists, overwrite it
                var filePath = fileLocation + filename + DateTime.Now.ToString().Replace(":", "_");
                xlBook.SaveAs(filePath,
                    Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value,
                    Missing.Value,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value,
                    Missing.Value);


                System.Diagnostics.Debug.WriteLine("Status: Complete. " + DateTime.Now.ToString("HH:mm:ss"));
            }
            catch (Exception ex) {
                string errMsg = "Error Saving Excel File - " + ex.Message;
                System.Diagnostics.Debug.WriteLine(errMsg);
                MessageBox.Show(errMsg);
            }

            finally {

                //if (xlSheet != null) {
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlSheet);
                //    xlSheet = null;

                //}
                //if (xlBook != null) {
                //    xlBook.Close(Type.Missing, Type.Missing, Type.Missing);
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlBook);
                //    xlBook = null;

                //}

                //if (xlApp != null) {
                //    xlApp.Quit();
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(xlApp);
                //    xlApp = null;
                //}




                //must release each object that referenced a COM object
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheet);
                xlSheet = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                xlBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBooks);
                xlBooks = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                xlApp = null;

                //await KillExcelAsync();

            }
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

            }
        }
    }
}
