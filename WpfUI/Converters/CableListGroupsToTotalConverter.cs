using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfUI.Converters
{
    public class CableListGroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>) {
                var items = (ReadOnlyObservableCollection<Object>)value;
                var itemsType = items.GetType().ToString();

            NextGroupLevel:
               try{
                    int total = 0;
                    foreach (ICable groupItem in items) {

                        if (groupItem.TypeModel.Conductors == 1) {
                            total += groupItem.QtyParallel * 3;
                        }
                        else {
                            total += groupItem.QtyParallel;
                        }
                    }
                    return total.ToString();
                }
                catch{
                    foreach (CollectionViewGroup cvg in items) {
                        items = cvg.Items;
                        goto NextGroupLevel;
                    }
                }

                //try{ 
                //    int total = 0;
                //    foreach (ICable groupItem in items) {

                //        if (groupItem.TypeModel.Conductors == 1) {
                //            total += groupItem.QtyParallel * 3;
                //        }
                //        else {
                //            total += groupItem.QtyParallel;
                //        }
                //    }
                //    return total.ToString();
                //}
                //catch {
                //    foreach (CollectionViewGroup cvg in items) {
                //        int total = 0;
                //        foreach (ICable groupItem in cvg.Items) {

                //            if (groupItem.TypeModel.Conductors == 1) {
                //                total += groupItem.QtyParallel * 3;
                //            }
                //            else {
                //                total += groupItem.QtyParallel;
                //            }
                //        }
                //        return total.ToString();
                //    }
                //}
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
