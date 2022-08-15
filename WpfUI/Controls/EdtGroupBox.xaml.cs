using EDTLibrary.Models;
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

namespace WpfUI.Controls;
/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class EdtGroupBox : UserControl
{




    public string Header
    {
        get { return (string)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(EdtGroupBox), new PropertyMetadata("GroupBox Header"));



    public Brush HeaderForeGround
    {
        get { return (Brush)GetValue(HeaderForeGroundProperty); }
        set { SetValue(HeaderForeGroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderForeGround.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderForeGroundProperty =
        DependencyProperty.Register("HeaderForeGround", typeof(Brush), typeof(EdtGroupBox), new PropertyMetadata(Brushes.Black));



    public Brush HeaderBackground
    {
        get { return (Brush)GetValue(HeaderBackgroundProperty); }
        set { SetValue(HeaderBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderBackgroundProperty =
        DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(EdtGroupBox), new PropertyMetadata(Brushes.LightBlue));



    public Brush ContentBackground
    {
        get { return (Brush)GetValue(ContentBackgroundProperty); }
        set { SetValue(ContentBackgroundProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ContentBackground.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContentBackgroundProperty =
        DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(EdtGroupBox), new PropertyMetadata(Brushes.Transparent));




    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(EdtGroupBox), new PropertyMetadata(new object()));




    public EdtGroupBox()
    {
        InitializeComponent();
    }
}
