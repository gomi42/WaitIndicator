using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WaitIndicator
{
    public abstract class WaitIndicator : FrameworkElement
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

        public int Shapes
        {
            get { return (int)GetValue(ShapesProperty); }
            set { SetValue(ShapesProperty, value); }
        }

        public static readonly DependencyProperty ShapesProperty =
            DependencyProperty.Register("Shapes", typeof(int), typeof(WaitIndicator), new PropertyMetadata(8, ShapesChanged));

        private static void ShapesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnShapesChanged();
        }

        public double ShapesGap
        {
            get { return (double)GetValue(ShapesGapProperty); }
            set { SetValue(ShapesGapProperty, value); }
        }

        public static readonly DependencyProperty ShapesGapProperty =
            DependencyProperty.Register("ShapesGap", typeof(double), typeof(WaitIndicator), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsArrange));

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

        private void CreateShapes()
        {
            for (int i = 0; i < Shapes; i++)
            {
                AddNewShape();
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

        private void OnShapesChanged()
        {
            RemoveStoryboard();

            if (Shapes < VisualChildrenCount)
            {
                while (VisualChildrenCount > Shapes)
                {
                    children.RemoveAt(0);
                }
            }
            else
            {
                while (VisualChildrenCount < Shapes)
                {
                    AddNewShape();
                }
            }

            TryCreateAndRunStoryboard();
            InvalidateArrange();
        }

        private void AddNewShape()
        {
            var shape = CreateShape();
            shape.Fill = Fill;
            children.Add(shape);
        }

        protected abstract Shape CreateShape();

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
            if (Visibility == Visibility.Visible && Duration.TotalMilliseconds > 0 && Shapes > 0)
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
