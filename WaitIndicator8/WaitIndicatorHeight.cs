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
            DependencyProperty.Register("ShapeHeight",
                                        typeof(double),
                                        typeof(WaitIndicatorHeight),
                                        new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsArrange, ShapesHeightChanged, CoerceShapeHeightChanged));

        private static void ShapesHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        private static object CoerceShapeHeightChanged(DependencyObject d, object obj)
        {
            var val = (double)obj;

            if (val < 0)
            {
                val = 0;
            }
            else
            if (val > 95)
            {
                val = 95;
            }

            return val;
        }

    }
}
