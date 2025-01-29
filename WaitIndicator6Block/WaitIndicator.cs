using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace WaitIndicator6Block
{
    public class WaitIndicator : FrameworkElement
    {
        private UIElementCollection children;
        private Storyboard storyboard;

        public WaitIndicator()
        {
            children = CreateUIElementCollection(null);

            CreateShapes();

            IsVisibleChanged += OnIsVisibleChanged;
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(WaitIndicator), new PropertyMetadata(Brushes.Gray, FillChanged));

        private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnFillChanged();
        }

        public int Segments
        {
            get { return (int)GetValue(SegmentsProperty); }
            set { SetValue(SegmentsProperty, value); }
        }

        public static readonly DependencyProperty SegmentsProperty =
            DependencyProperty.Register("Segments", typeof(int), typeof(WaitIndicator), new PropertyMetadata(8, SegmentsChanged));

        private static void SegmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnSegmentsNumberChanged();
        }

        public double SegmentGap
        {
            get { return (double)GetValue(SegmentGapProperty); }
            set { SetValue(SegmentGapProperty, value); }
        }

        public static readonly DependencyProperty SegmentGapProperty =
            DependencyProperty.Register("SegmentGap", typeof(double), typeof(WaitIndicator), new PropertyMetadata(10.0, SegmentGapChanged));

        private static void SegmentGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnSegmentsChanged();
        }

        public double SegmentHeight
        {
            get { return (double)GetValue(SegmentHeightProperty); }
            set { SetValue(SegmentHeightProperty, value); }
        }

        public static readonly DependencyProperty SegmentHeightProperty =
            DependencyProperty.Register("SegmentHeight", typeof(double), typeof(WaitIndicator), new PropertyMetadata(30.0, SegmentHeightChanged));

        private static void SegmentHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnSegmentsChanged();
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(WaitIndicator), new PropertyMetadata(TimeSpan.FromMilliseconds(1600), TimeChanged));

        private static void TimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnTimeChanged();
        }

        private UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, logicalParent);
        }

        protected override int VisualChildrenCount => children.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (children == null)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return children[index];
        }

        protected override IEnumerator LogicalChildren => children.GetEnumerator();

        protected override Size MeasureOverride(Size constraint)
        {
            if (VisualChildrenCount == 0)
            {
                return (constraint);
            }

            double width = constraint.Width;

            if (constraint.Height < constraint.Width)
            {
                width = constraint.Height;
            }

            return new Size(width, width);
        }

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
                var shape = (Path)children[i];

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
            var angleWithGap = 2 * Math.PI / numShapes * (100 - SegmentGap) / 100;

            double innerRadius = outerRadius * (100 - SegmentHeight) / 100;

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

            var a = Math.Cos(angle / 2)  / Math.Sin(angle / 2);
            var b = y1 + a * x1;

            var aq = 1 + a * a;
            var bq = 2 * a *b;
            var cq = b * b - innerRadius*innerRadius;

            var xq1 = (-bq + Math.Sqrt(bq*bq - 4*aq*cq)) / (2 * aq);
            var xq2 = (-bq - Math.Sqrt(bq*bq - 4*aq*cq)) / (2 * aq);

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

        private void CreateShapes()
        {
            for (int i = 0; i < Segments; i++)
            {
                CreateShape();
            }
        }

        private void CreateStoryBoard()
        {
            double duration = Duration.TotalMilliseconds;
            double step = duration / VisualChildrenCount;

            storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            for (int i = 1; i < VisualChildrenCount; i++)
            {
                var shape = (Shape)children[i];

                var animation2 = new DoubleAnimation((double)step / duration * i, 0.0, new Duration(TimeSpan.FromMilliseconds(i * step)));
                Storyboard.SetTarget(animation2, shape);
                Storyboard.SetTargetProperty(animation2, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation2);
            }

            for (int i = 0; i < VisualChildrenCount; i++)
            {
                var shape = (Shape)children[i];

                var animation = new DoubleAnimation(1.0, (double)step / duration * i, new Duration(TimeSpan.FromMilliseconds(duration - i * step)));
                animation.BeginTime = TimeSpan.FromMilliseconds(i * step);
                Storyboard.SetTarget(animation, shape);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation);
            }
        }

        private void OnFillChanged()
        {
            foreach (var shape in children)
            {
                ((Shape)shape).Fill = Fill;
            }
        }

        private void OnSegmentsNumberChanged()
        {
            RemoveStoryboard();

            if (Segments < VisualChildrenCount)
            {
                while (VisualChildrenCount > Segments)
                {
                    children.RemoveAt(0);
                }
            }
            else
            {
                while (VisualChildrenCount < Segments)
                {
                    CreateShape();
                }
            }

            TryCreateAndRunStoryboard();
            InvalidateArrange();
        }

        private void OnSegmentsChanged()
        {
            RemoveStoryboard();

            while (VisualChildrenCount > 0)
            {
                children.RemoveAt(0);
            }

            CreateShapes();
            TryCreateAndRunStoryboard();
            InvalidateArrange();
        }

        private void CreateShape()
        {
            var shape = new Path();
            shape.Fill = Fill;
            shape.RenderTransformOrigin = new Point(0, 0);
            shape.Stretch = Stretch.None;
            children.Add(shape);
        }

        private void OnTimeChanged()
        {
            RemoveStoryboard();
            TryCreateAndRunStoryboard();
        }

        private void RemoveStoryboard()
        {
            if (storyboard != null)
            {
                storyboard.Stop(this);
                storyboard.Remove(this);
                storyboard = null;
            }
        }

        private void TryCreateAndRunStoryboard()
        {
            if (Visibility == Visibility.Visible && Duration.TotalMilliseconds > 0 && Segments > 0)
            {
                CreateStoryBoard();
                storyboard.Begin(this, true);
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                TryCreateAndRunStoryboard();
            }
            else
            {
                RemoveStoryboard();
            }
        }
    }
}
