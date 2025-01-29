using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class SegmentWaitIndicator : WaitIndicator
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (VisualChildrenCount == 0)
            {
                return arrangeSize;
            }

            int numShapes = VisualChildrenCount;

            if (numShapes == 0)
            {
                return arrangeSize;
            }

            double angle = 0;
            double width = arrangeSize.Width;

            if (arrangeSize.Height < arrangeSize.Width)
            {
                width = arrangeSize.Height;
            }

            var radius = width / 2;
            var geometry = CreatePath(radius);

            for (int i = 0; i < numShapes; i++)
            {
                var shape = (Path)GetVisualChild(i);

                var sin = Math.Sin(angle);
                var cos = Math.Cos(angle);

                var x = cos * radius + radius;
                var y = sin * radius + radius;

                var degrees = angle * 180 / Math.PI + 90;
                shape.RenderTransform = new RotateTransform(degrees);

                shape.Data = geometry;
                shape.Arrange(new Rect(x, y, width, radius));
                shape.Opacity = (numShapes - i) / (double)numShapes;

                angle += 2 * Math.PI / numShapes;
            }

            return arrangeSize;
        }

        private Geometry CreatePath(double outerRadius)
        {
            int numShapes = VisualChildrenCount;
            var angle = 2 * Math.PI / numShapes;
            var angleWithGap = 2 * Math.PI / numShapes * (100 - ShapesGap) / 100;

            double innerRadius = outerRadius * (100 - ShapeHeight) / 100;

            var sin = Math.Sin(angleWithGap / 2);
            var cos = Math.Cos(angleWithGap / 2);

            var x1 = -sin * outerRadius;
            var y1 = cos * outerRadius;

            var x2 = -x1;
            var y2 = y1;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathGeometry.Figures = pathFigureCollection;
            PathFigure pathFigure;
            PathSegmentCollection pathSegmentCollection;
            pathFigure = new PathFigure();
            pathFigureCollection.Add(pathFigure);
            pathFigure.IsClosed = true;
            pathFigure.StartPoint = new Point(x1, outerRadius - y1);

            pathSegmentCollection = new PathSegmentCollection();
            pathFigure.Segments = pathSegmentCollection;

            ArcSegment arcSegment;
            arcSegment = new ArcSegment();
            pathSegmentCollection.Add(arcSegment);

            arcSegment.Point = new Point(x2, outerRadius - y2);
            arcSegment.Size = new Size(outerRadius, outerRadius);
            arcSegment.SweepDirection = SweepDirection.Clockwise;
            arcSegment.IsLargeArc = false;

            /////////

            var a = Math.Cos(angle / 2) / Math.Sin(angle / 2);
            var b = y1 + a * x1;

            var aq = 1 + a * a;
            var bq = 2 * a * b;
            var cq = b * b - innerRadius * innerRadius;

            var xq1 = (-bq + Math.Sqrt(bq * bq - 4 * aq * cq)) / (2 * aq);
            var yq = Math.Sqrt(innerRadius * innerRadius - xq1 * xq1);

            x1 = -xq1;
            y1 = yq;

            x2 = xq1;
            y2 = yq;

            LineSegment lineSegment;
            lineSegment = new LineSegment();
            pathSegmentCollection.Add(lineSegment);
            lineSegment.Point = new Point(x2, outerRadius - y1);

            arcSegment = new ArcSegment();
            pathSegmentCollection.Add(arcSegment);

            arcSegment.Point = new Point(x1, outerRadius - y2);
            arcSegment.Size = new Size(innerRadius, innerRadius);
            arcSegment.SweepDirection = SweepDirection.Counterclockwise;
            arcSegment.IsLargeArc = false;

            pathGeometry.Freeze();

            return pathGeometry;
        }

        protected override Shape CreateShape()
        {
            var shape = new Path();
            shape.RenderTransformOrigin = new Point(0, 0);
            shape.Stretch = Stretch.None;

            return shape;
        }
    }
}
