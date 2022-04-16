using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfUI.Converters;

class DataGridGroupTemplateSelector : DataTemplateSelector
{
	public DataTemplate cableListTypeTemplate { get; set; }
	public DataTemplate cableListSizeTemplate { get; set; }
	public DataTemplate cableListUsageTypeTemplate { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		ContentPresenter contentPresenter = container as ContentPresenter;

		if (contentPresenter != null) {
			DataGrid dataGrid = GetDataGrid(contentPresenter);
			CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
			
			int groupDescCount = collectionView.GroupDescriptions.Count;	// get count of groupDescriptions
			int level;
			CollectionViewGroup cvg = contentPresenter.Content as CollectionViewGroup;

			
			level = GetLevel(cvg, groupDescCount - 1); // detect level from depth to bottom

			// get PropertyName from PropertyGroupDescription of level and select Template to PropertyName
			string propertyName = (collectionView.GroupDescriptions[level] as PropertyGroupDescription).PropertyName;

			DataTemplate result = null;
            //DataTemplate cableListTypeTemplate = (DataTemplate)Application.Current.Resources["TypeTemplate"];
            //DataTemplate cableListSizeTemplate = (DataTemplate)Application.Current.Resources["SizeTemplate"];
            //DataTemplate cableListUsageTypeTemplate = (DataTemplate)Application.Current.Resources["UsageTemplate"];

			switch (propertyName) {
				case "Type":
					result = cableListTypeTemplate;
					break;
				case "Size":
					result = cableListSizeTemplate;
					break;
				case "UsageType":
					result = cableListUsageTypeTemplate;
					break;
			}
			return result;
		}
		return base.SelectTemplate(item, container);
	}

	private static DataGrid GetDataGrid(DependencyObject parent)
	{
		if (parent != null) {
			if (parent.GetType() == typeof(DataGrid)) {
				return (DataGrid)parent;
			}
			return GetDataGrid(VisualTreeHelper.GetParent(parent));
		}
		else {
			return null;
		}
	}

	private static int GetLevel(CollectionViewGroup cvg, int inLevel)
	{
		if (cvg != null) {
			if (cvg.IsBottomLevel) {
				return inLevel;
			}
			else {
				return GetLevel((CollectionViewGroup)cvg.Items[0], inLevel-1);
			}
		}
		else {
			return inLevel;
		}
	}
}

