using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using WpfUI.ViewModels;
using WpfUI.Views.SettingsSubViews;

namespace WpfUI.Views
{
    /// <summary>
    /// Interaction logic for ProjectSettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel settingsVm { get { return DataContext as SettingsViewModel; } }

        public SettingsView() {
            InitializeComponent();
        }

        private void dgdTableSetting_MouseLeave(object sender, MouseEventArgs e)
        {
        }


        /// <summary>
        /// Changes checkbox columsn to Template checkbox columsn for single clickable check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgdTableSetting_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.Column is DataGridCheckBoxColumn && !e.Column.IsReadOnly) {
                var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
                checkboxFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                checkboxFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                checkboxFactory.SetBinding(ToggleButton.IsCheckedProperty, new Binding(e.PropertyName) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                e.Column = new DataGridTemplateColumn {
                    Header = e.Column.Header,
                    CellTemplate = new DataTemplate {
                        VisualTree = checkboxFactory
                    },
                    SortMemberPath = e.Column.SortMemberPath
                };
            }
            if (e.PropertyName == "Id") {
                e.Cancel = true;

            }

        }

        private void btnGeneralSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //GeneralSettingsView settingView = new GeneralSettingsView();
            //settingView.DataContext = this.DataContext;
            //ccSettingPage.Content = settingView;
            settingsVm.SelectedSettingView = new GeneralSettingsView();
        }

        private void btnDeveloperSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //DeveloperSettingsView settingView = new DeveloperSettingsView();
            //settingView.DataContext = this.DataContext;
            //ccSettingPage.Content = settingView;
            settingsVm.SelectedSettingView = new DeveloperSettingsView();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //CableSettingsView settingView = new CableSettingsView();
            //settingView.DataContext = this.DataContext;
            //ccSettingPage.Content = settingView;
            settingsVm.SelectedSettingView = new CableSettingsView();
        }
    }
}
