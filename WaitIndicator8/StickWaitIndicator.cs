using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class StickWaitIndicator : WaitIndicator
    {
        public double ShapeHeight
        {
            get { return (double)GetValue(ShapeHeightProperty); }
            set { SetValue(ShapeHeightProperty, value); }
        }

        public static readonly DependencyProperty ShapeHeightProperty =
            DependencyProperty.Register("ShapeHeight", typeof(double), typeof(WaitIndicator), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (VisualChildrenCount == 0)
            {
                return arrangeSize;
            }

            int numShapes = VisualChildrenCount;
            double angle = 0;
            double width = arrangeSize.Width;

            if (arrangeSize.Height < arrangeSize.Width)
            {
                width = arrangeSize.Height;
            }

            var innerRadius = (width - width * ShapeHeight / 100) / 2;
            var shapeWidth = Math.Truncate(2 * Math.PI * innerRadius / numShapes * (100 - ShapesGap) / 100);
            var radius = width / 2;
            var shapeHeight = radius - innerRadius;

            for (int i = 0; i < numShapes; i++)
            {
                var shape = (Shape)GetVisualChild(i);

                var sin = Math.Sin(angle);
                var cos = Math.Cos(angle);

                var x = cos * radius + radius;
                var y = sin * radius + radius;

                var tg = new TransformGroup();
                var trans = new TranslateTransform(-shapeWidth / 2, 0);
                var degrees = angle * 180 / Math.PI + 90;
                var rt = new RotateTransform(degrees);
                tg.Children.Add(trans);
                tg.Children.Add(rt);
                shape.RenderTransform = tg;

                shape.Arrange(new Rect(x, y, shapeWidth, shapeHeight));

                shape.Opacity = (numShapes - i) / (double)numShapes;
                angle += 2 * Math.PI / numShapes;
            }

            return arrangeSize;
        }

        protected override Shape CreateShape()
        {
            var shape = new Rectangle();
            shape.RenderTransformOrigin = new Point(0, 0);

            return shape;
        }
    }
}
