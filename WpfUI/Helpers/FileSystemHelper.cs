using Microsoft.Win32;

namespace WpfUI.Helpers
{
    public class FileSystemHelper
    {
        public static string SelectFilePath(string initialDirectory = "c:\\", string filterText = "EDT files (*.db)|*.db|All files (*.*)|*.*")
        {
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = initialDirectory;
                openFileDialog.Filter = filterText;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                
                if (openFileDialog.ShowDialog() == true) {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            
            return filePath;
        }
    }
}
