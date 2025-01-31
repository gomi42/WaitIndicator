using System;
using System.Windows;
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

        protected override Shape CreateShape()
        {
            var shape = new Ellipse();

            return shape;
        }
    }
}
