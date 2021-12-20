using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfUI.Helpers
{
    public class FileSystemHelper
    {
        public static string SelectFile(string initialDirectory = "c:\\", string filterText= "EDT files (*.db)|*.db|All files (*.*)|*.*")
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = initialDirectory;
                openFileDialog.Filter = filterText;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;                    
                }
            }
            return filePath;
        }
    }
}
