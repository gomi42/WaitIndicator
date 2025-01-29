using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WaitIndicator7Sticks
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

        public int Sticks
        {
            get { return (int)GetValue(SticksProperty); }
            set { SetValue(SticksProperty, value); }
        }

        public static readonly DependencyProperty SticksProperty =
            DependencyProperty.Register("Sticks", typeof(int), typeof(WaitIndicator), new PropertyMetadata(8, SticksChanged));

        private static void SticksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnSticksChanged();
        }

        public double StickHeight
        {
            get { return (double)GetValue(StickHeightProperty); }
            set { SetValue(StickHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StickHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StickHeightProperty =
            DependencyProperty.Register("StickHeight", typeof(double), typeof(WaitIndicator), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double SticksGap
        {
            get { return (double)GetValue(SticksGapProperty); }
            set { SetValue(SticksGapProperty, value); }
        }

        public static readonly DependencyProperty SticksGapProperty =
            DependencyProperty.Register("SticksGap", typeof(double), typeof(WaitIndicator), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsArrange));

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
            double angle = 0;
            double width = arrangeSize.Width;

            if (arrangeSize.Height < arrangeSize.Width)
            {
                width = arrangeSize.Height;
            }

            var innerRadius = (width - width * StickHeight / 100) / 2;
            var shapeWidth = Math.Truncate(2 * Math.PI * innerRadius / numShapes * (100 - SticksGap) / 100);
            var radius = width / 2;
            var shapeHeight = radius - innerRadius;

            for (int i = 0; i < numShapes; i++)
            {
                var shape = (Shape)children[i];

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

        private void CreateShapes()
        {
            for (int i = 0; i < Sticks; i++)
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

        private void OnSticksChanged()
        {
            RemoveStoryboard();

            if (Sticks < VisualChildrenCount)
            {
                while (VisualChildrenCount > Sticks)
                {
                    children.RemoveAt(0);
                }
            }
            else
            {
                while (VisualChildrenCount < Sticks)
                {
                    CreateShape();
                }
            }

            TryCreateAndRunStoryboard();
            InvalidateArrange();
        }

        private void OnStickHeightChanged()
        {
            InvalidateArrange();
        }

        private void CreateShape()
        {
            var shape = new Rectangle();
            shape.Fill = Fill;
            shape.RenderTransformOrigin = new Point(0, 0);
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
            if (Visibility == Visibility.Visible && Duration.TotalMilliseconds > 0 && Sticks > 0)
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
