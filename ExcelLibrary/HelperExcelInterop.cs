using System;
using System.Collections.Generic;
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
        public static void WriteToExcel(string fileLocation = "C:\\temp\\", string filename = "Edt Excel Test")
        {
            object oMissing = Missing.Value;

            Excel.Application excelApp = null;
            Excel.Range range = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            int worksheetCount = 0;

            try {
                //Initialize

                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;
                excelApp.Visible = true;
                //excelApp.UserControl = false;
                //excelApp.Calculation = Excel.XlCalculation.xlCalculationManual;
                //excelApp.ScreenUpdating = false;

                //Create 
                workbook = excelApp.Workbooks.Add();
                worksheetCount = workbook.Sheets.Count;
                worksheet = workbook.Sheets["Sheet1"];

                for (int i = 0; i < 10; i++) {
                    int rowNum = i + 1;

                    for (int j = 0; j < 10; j++) {
                        int colNum = j + 1;

                        worksheet.Cells[rowNum, colNum] = $"{rowNum}, {colNum}";
                    }
                }

                //enable
                //excelApp.Calculation = Excel.XlCalculation.xlCalculationManual;
                //excelApp.ScreenUpdating = true;

                //save Workbook - if file exists, overwrite it
                var filePath = fileLocation + filename + DateTime.Now.ToString().Replace(":", "_");
                workbook.SaveAs(filePath,
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
                if (workbook != null) {
                    //close workbook
                    //workbook.Close();

                    //release all resources
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
                }

                if (excelApp != null) {
                    //close Excel
                    //excelApp.Quit();

                    //release all resources
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
                }
            }
        }
    }
}
