using System;
using System.Windows;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class StickWaitIndicator : WaitIndicatorHeight
    {
        protected override Size BeginArrangeShapes(double radius, int numShapes)
        {
            var innerRadius = radius - radius * ShapeHeight / 100;
            var shapeWidth = Math.Truncate(2 * Math.PI * innerRadius / numShapes * (100 - ShapesGap) / 100);
            var shapeHeight = radius - innerRadius;

            return new Size(shapeWidth, shapeHeight);
        }

        protected override Shape CreateShape()
        {
            var shape = new Rectangle();

            return shape;
        }
    }
}
