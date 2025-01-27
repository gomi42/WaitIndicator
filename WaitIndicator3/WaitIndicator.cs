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

namespace WaitIndicator3
{
    public class WaitIndicator : Control
    {
        List<FrameworkElement> shapes = new List<FrameworkElement>();
        private Storyboard storyboard;

        public WaitIndicator()
        {
            SizeChanged += OnSizeChanged;
            IsVisibleChanged += OnIsVisibleChanged;
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RemoveStoryboard();

            shapes = new List<FrameworkElement>();

            var canvas = GetTemplateChild("Part_ShapeContainer") as Canvas;

            if (canvas == null)
            {
                return;
            }

            var shapeIndex = 0;

            foreach (var shape in canvas.Children)
            {
                shapes.Add((FrameworkElement)shape);
                shapeIndex++;
            }

            TryCreateAndRunStoryboard();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetAllPositions();
        }

        private void SetAllPositions()
        {
            int numShapes = shapes.Count;
            double angle = 0;
            double width = ActualWidth;

            if (ActualHeight < ActualWidth)
            {
                width = ActualHeight;
            }

            var radius = numShapes * width / (2 * (numShapes + Math.PI));
            var shapeWidth = Math.Truncate(2 * Math.PI * radius / numShapes * 0.85);
            radius = (width - shapeWidth) / 2;

            for (int i = 0; i < numShapes; i++)
            {
                var shape = shapes[i];

                shape.Width = shapeWidth;
                shape.Height = shapeWidth;

                var sin = Math.Sin(angle);
                var cos = Math.Cos(angle);

                var x = cos * radius + radius;
                var y = sin * radius + radius;
                Canvas.SetLeft(shape, x);
                Canvas.SetTop(shape, y);

                angle += 2 * Math.PI / numShapes;
                shape.Opacity = (numShapes - i) / (double)numShapes;
            }
        }

        private void CreateStoryBoard()
        {
            int step = Time / shapes.Count;

            storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;

            for (int i = 1; i < shapes.Count; i++)
            {
                var shape = shapes[i];

                var animation2 = new DoubleAnimation((double)step / Time * i, 0.0, new Duration(TimeSpan.FromMilliseconds(i * step)));
                Storyboard.SetTarget(animation2, shape);
                Storyboard.SetTargetProperty(animation2, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation2);
            }

            for (int i = 0; i < shapes.Count; i++)
            {
                var shape = shapes[i];

                var animation = new DoubleAnimation(1.0, (double)step / Time * i, new Duration(TimeSpan.FromMilliseconds(Time - i * step)));
                animation.BeginTime = TimeSpan.FromMilliseconds(i * step);
                Storyboard.SetTarget(animation, shape);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation);
            }
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
            if (Visibility == Visibility.Visible && Time > 0 && shapes.Count > 0)
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
