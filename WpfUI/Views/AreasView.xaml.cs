﻿using EDTLibrary;
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

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for AreasView.xaml
    /// </summary>
    public partial class AreasView : UserControl
    {
        public AreasView()
        {
            InitializeComponent();
        }
        private void txtAreaTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtAreaTag.Text == "" || txtAreaTag.Text == GlobalConfig.EmptyTag) txtAreaTag.Text = "";
        }

        private void txtAreaTag_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAreaTag.Text == "") txtAreaTag.Text = GlobalConfig.EmptyTag;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}