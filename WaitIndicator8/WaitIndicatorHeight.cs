using System.Windows;

namespace WaitIndicator
{
    internal abstract class WaitIndicatorHeight : WaitIndicator
    {
        public double ShapeHeight
        {
            get { return (double)GetValue(ShapeHeightProperty); }
            set { SetValue(ShapeHeightProperty, value); }
        }

        public static readonly DependencyProperty ShapeHeightProperty =
            DependencyProperty.Register("ShapeHeight", typeof(double), typeof(WaitIndicatorHeight), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsArrange));

    }
}
