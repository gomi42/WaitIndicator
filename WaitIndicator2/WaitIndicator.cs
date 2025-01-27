using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WaitIndicator2
{
    internal class WaitIndicator : Control
    {
        List<Shape> shapes;

        public WaitIndicator()
        {
            SizeChanged += OnSizeChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            shapes = new List<Shape>();

            var shapeIndex = 0;

            while (true)
            {
                var shape = GetTemplateChild($"Part_Ellipse{shapeIndex}") as Shape;

                if (shape == null)
                {
                    break;
                }

                shapes.Add(shape);
                shapeIndex++;
            }
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
    }
}
