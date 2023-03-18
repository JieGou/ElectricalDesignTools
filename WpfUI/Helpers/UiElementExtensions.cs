using System.Windows;
using System.Windows.Media;

namespace WpfUI.Views.Electrical.LoadListSubViews;

public static class UiElementExtensions
{
    public static T FindVisualParent<T>(this UIElement element)
        where T : UIElement
    {
        var currentElement = element;

        while (currentElement != null) {
            if (currentElement is T correctlyTyped) {
                return correctlyTyped;
            }

            currentElement = VisualTreeHelper.GetParent(currentElement) as UIElement;
        }

        return null;
    }
}



