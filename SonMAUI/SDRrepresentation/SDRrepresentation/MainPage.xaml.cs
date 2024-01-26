using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace DrawScatterPlotMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var graphicsView = new GraphicsView();
            graphicsView.PaintSurface += OnGraphicsViewPaintSurface;

            Content = new StackLayout
            {
                Children = { graphicsView }
            };
        }

        private void OnGraphicsViewPaintSurface(object sender, DrawEventArgs args)
        {
            var canvas = args.DrawingCanvas;

            // Clear the canvas
            canvas.Clear(Color.White);

            // Sample data for scatter plot
            var dataPoints = new[]
            {
                new DataPoint(50, 100),
                new DataPoint(100, 150),
                new DataPoint(150, 200),
                // Add more data points as needed
            };

            // Create a paint object for drawing points
            using (var pointPaint = new Paint())
            {
                pointPaint.Color = Colors.Blue;

                // Draw each data point as a circle
                foreach (var dataPoint in dataPoints)
                {
                    canvas.DrawCircle(dataPoint.X, dataPoint.Y, 5, pointPaint);
                }
            }
        }

        private struct DataPoint
        {
            public float X { get; }
            public float Y { get; }

            public DataPoint(float x, float y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
