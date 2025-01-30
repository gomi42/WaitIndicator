using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class DotWaitIndicator : WaitIndicator
    {
        protected override Size BeginArrangeShapes(double radius, int numShapes)
        {
            var dotCenterRadius = numShapes * radius / (numShapes + Math.PI);
            var shapeWidth = Math.Truncate(2 * Math.PI * dotCenterRadius / numShapes * (100 - ShapesGap) / 100);

            return new Size(shapeWidth, shapeWidth);
        }

        protected override void PrepareArrangeShape(Shape shape, double angle, Size shapeSize)
        {
            var transformGroup = new TransformGroup();
            var translate = new TranslateTransform(-shapeSize.Width / 2, 0);
            var rotate = new RotateTransform(angle);
            transformGroup.Children.Add(translate);
            transformGroup.Children.Add(rotate);

            shape.RenderTransform = transformGroup;
        }

        protected override Shape CreateShape()
        {
            var shape = new Ellipse();
            shape.RenderTransformOrigin = new Point(0, 0);

            return shape;
        }
    }
}
