using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class DotWaitIndicator : WaitIndicator
    {
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

            var radius = numShapes * width / (2 * (numShapes + Math.PI));
            var shapeWidth = Math.Truncate(2 * Math.PI * radius / numShapes * (100 - ShapesGap) / 100);
            radius = width / 2;

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

                shape.Arrange(new Rect(x, y, shapeWidth, shapeWidth));

                shape.Opacity = (numShapes - i) / (double)numShapes;
                angle += 2 * Math.PI / numShapes;
            }

            return arrangeSize;
        }

        protected override Shape CreateShape()
        {
            var shape = new Ellipse();
            shape.RenderTransformOrigin = new Point(0, 0);

            return shape;
        }
    }
}
