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

namespace WpfUI.Views.Electrical.MjeqSubviews;
/// <summary>
/// Interaction logic for TransformerGraphicView.xaml
/// </summary>
public partial class TransformerGraphicView : UserControl
{
    public TransformerGraphicView()
    {
        InitializeComponent();
    }

    private void txtImpedance_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return) {
            BindingExpression binding = txtImpedance.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }
    }
}
