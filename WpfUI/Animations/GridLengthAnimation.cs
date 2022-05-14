using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfUI.Animations;

public class GridLengthAnimation : AnimationTimeline
{
    static GridLengthAnimation()
    {
        FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
        ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation)); 
    }

    public GridLength From
    {
        get { return (GridLength)GetValue(FromProperty); }
        set { SetValue(FromProperty, value); }
    }

    public static readonly DependencyProperty FromProperty;
    public GridLength To
    {
        get { return (GridLength)GetValue(ToProperty); }
        set { SetValue(ToProperty, value); }
    }

    public static readonly DependencyProperty ToProperty;

    public override Type TargetPropertyType
    {
        get { return typeof(GridLength); }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new GridLengthAnimation();
    }

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
    {
        double fromValue = ((GridLength)GetValue(GridLengthAnimation.FromProperty)).Value;
        double toValue = ((GridLength)GetValue(GridLengthAnimation.ToProperty)).Value;

        //double fromValue = this.From.Value;
        //double toValue = this.To.Value;

        if (fromValue > toValue) {
            return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromValue - toValue) + toValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
        }
        else {
            return new GridLength((animationClock.CurrentProgress.Value) * (toValue - fromValue) + fromValue, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
        }
    }
}
