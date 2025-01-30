using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WaitIndicator
{
    internal class SegmentWaitIndicator : WaitIndicatorHeight
    {
        Geometry currentGeometry;

        protected override Size BeginArrangeShapes(double radius, int numShapes)
        {
            currentGeometry = CreatePath(radius);
            return new Size(currentGeometry.Bounds.Width, currentGeometry.Bounds.Height);
        }

        protected override void PrepareArrangeShape(Shape shape, double angle, Size shapeSize)
        {
            ((Path)shape).Data = currentGeometry;
            shape.RenderTransform = new RotateTransform(angle);
        }

        protected override void EndArrangeShapes()
        {
            currentGeometry = null;
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
