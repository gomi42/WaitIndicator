using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WaitIndicator4
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

        public int Dots
        {
            get { return (int)GetValue(DotsProperty); }
            set { SetValue(DotsProperty, value); }
        }

        public static readonly DependencyProperty DotsProperty =
            DependencyProperty.Register("Dots", typeof(int), typeof(WaitIndicator), new PropertyMetadata(8, DotsChanged));

        private static void DotsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WaitIndicator)d).OnDotsChanged();
        }

        public int Time
        {
            get { return (int)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(int), typeof(WaitIndicator), new PropertyMetadata(1600, TimeChanged));

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

            var radius = numShapes * width / (2 * (numShapes + Math.PI));
            var shapeWidth = Math.Truncate(2 * Math.PI * radius / numShapes * 0.85);
            radius = (width - shapeWidth) / 2;

            for (int i = 0; i < numShapes; i++)
            {
                var shape = (Shape)children[i];

                var sin = Math.Sin(angle);
                var cos = Math.Cos(angle);

                var x = cos * radius + radius;
                var y = sin * radius + radius;

                shape.Arrange(new Rect(x, y, shapeWidth, shapeWidth));

                shape.Opacity = (numShapes - i) / (double)numShapes;
                angle += 2 * Math.PI / numShapes;
            }

            return arrangeSize;
        }

        private void CreateShapes()
        {
            for (int i = 0; i < Dots; i++)
            {
                CreateShape();
            }
        }

        private void CreateStoryBoard()
        {
            int step = Time / VisualChildrenCount;

            storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            for (int i = 1; i < VisualChildrenCount; i++)
            {
                var shape = (Shape)children[i];

                var animation2 = new DoubleAnimation((double)step / Time * i, 0.0, new Duration(TimeSpan.FromMilliseconds(i * step)));
                Storyboard.SetTarget(animation2, shape);
                Storyboard.SetTargetProperty(animation2, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation2);
            }

            for (int i = 0; i < VisualChildrenCount; i++)
            {
                var shape = (Shape)children[i];

                var animation = new DoubleAnimation(1.0, (double)step / Time * i, new Duration(TimeSpan.FromMilliseconds(Time - i * step)));
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

        private void OnDotsChanged()
        {
            RemoveStoryboard();

            if (Dots < VisualChildrenCount)
            {
                while (VisualChildrenCount > Dots)
                {
                    children.RemoveAt(0);
                }
            }
            else
            {
                while (VisualChildrenCount < Dots)
                {
                    CreateShape();
                }
            }

            TryCreateAndRunStoryboard();
            InvalidateArrange();
        }

        private void CreateShape()
        {
            var shape = new Ellipse();
            shape.Fill = Fill;
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
            if (Visibility == Visibility.Visible && Time > 0 && Dots > 0)
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
