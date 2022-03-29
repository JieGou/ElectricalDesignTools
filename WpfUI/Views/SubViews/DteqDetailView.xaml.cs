using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfUI.PopupWindows;

namespace WpfUI.Views.SubViews
{
    /// <summary>
    /// Interaction logic for DteqDetailView.xaml
    /// </summary>
    public partial class DteqDetailView : UserControl
    {
        public DteqDetailView()
        {
            InitializeComponent();
        }

        //uncomment below:

        //PdfViewer viewer = new PdfViewer();

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //LoadPdfViewer();
        }

        private void LoadPdfViewer()
        {
            //comment below
            PdfViewer viewer = new PdfViewer();

            viewer.Close();
            viewer = null;
            viewer = new PdfViewer();
            viewer.Title = "PREVIEW";

            //PDF
            string pdfFolder = "C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\CEC Tables\\";
            string pdfFile = "Appendix D Tables.pdf";
            string pdfSettings = "#toolbar=0&navpanes=0";
            viewer.ChromeViewer.Address = pdfFolder + pdfFile + pdfSettings;

            //viewer.pdfWebViewer.Navigate(pdfFolder + pdfFile + pdfSettings);

            //Img
            BitmapImage img = new BitmapImage();
            string imgFolder = "C:\\Users\\pdeau\\Google Drive\\Work\\Visual Studio Projects\\_EDT Tables\\CEC Tables\\Table Image Files\\";
            string imageFile = "CEC 2018 - Tables_Page_001.jpg";
            string imgPath = imgFolder + imageFile;
            img.BeginInit();
            img.UriSource = new Uri(imgPath);
            img.EndInit();
            //viewer.image.Source = img;

            viewer.Show();
            viewer.Focus();
        }
    }
}
