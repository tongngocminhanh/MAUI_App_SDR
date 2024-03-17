
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;
using Font = Microsoft.Maui.Graphics.Font;


namespace AppSDR
{
    public class GraphicsDrawable : BindableObject, IDrawable, INotifyPropertyChanged
    {
        public float rectangleWidth;
        public float rectangleSpacing;

        public int[][] Vectors
        {
            get => (int[][])GetValue(VectorsProperty);
            set => SetValue(VectorsProperty, value);
        }

        public static readonly BindableProperty VectorsProperty = BindableProperty.Create(nameof(Vectors), typeof(int[][]), typeof(GraphicsDrawable));

        public string[] GraphPara
        {
            get => (string[])GetValue(GraphParaProperty);
            set => SetValue(GraphParaProperty, value);
        }

        public static readonly BindableProperty GraphParaProperty = BindableProperty.Create(nameof(GraphPara), typeof(string[]), typeof(GraphicsDrawable));

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (GraphPara == null || GraphPara.Length < 7)
                return;

            string graphName = null;
            int? maxCycles = null;
            int? highlightTouch = null;
            string xAxisTitle = null;
            string yAxisTitle = null;
            int? minRange = null;
            int? maxRange = null;

            // Assign values if corresponding elements in GraphPara are not null or empty
            if (!string.IsNullOrEmpty(GraphPara[0])) graphName = GraphPara[0];
            if (!string.IsNullOrEmpty(GraphPara[1])) maxCycles = int.Parse(GraphPara[1]);
            if (!string.IsNullOrEmpty(GraphPara[2])) highlightTouch = int.Parse(GraphPara[2]);
            if (!string.IsNullOrEmpty(GraphPara[3])) xAxisTitle = GraphPara[3];
            if (!string.IsNullOrEmpty(GraphPara[4])) yAxisTitle = GraphPara[4];
            if (!string.IsNullOrEmpty(GraphPara[5])) minRange = int.Parse(GraphPara[5]);
            if (!string.IsNullOrEmpty(GraphPara[6])) maxRange = int.Parse(GraphPara[6]);

            DrawBackground(canvas, dirtyRect);
            DrawTitles(canvas, graphName, xAxisTitle, yAxisTitle, dirtyRect);
            DrawGraph(canvas, dirtyRect, maxCycles, highlightTouch, minRange, maxRange);
            DrawTicks(canvas, dirtyRect);
        }

        private void DrawBackground(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.WhiteSmoke;
            canvas.FillRectangle(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
        }

        private void DrawTitles(ICanvas canvas, string graphName, string xAxisTitle, string yAxisTitle, RectF dirtyRect)
        {
            float canvasHeight = dirtyRect.Height - 100;
            float x_canvas = (dirtyRect.Width - (Vectors.Length * (rectangleWidth + rectangleSpacing))) / 2;

         
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 20;

            canvas.DrawString($" {graphName}", x_canvas, 30, 200, 50, HorizontalAlignment.Center, VerticalAlignment.Top);
            canvas.DrawString($" {xAxisTitle}", x_canvas, canvasHeight, 200, 70, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            canvas.DrawString($" {yAxisTitle}", x_canvas - 200, canvasHeight - 200, 200, 70, HorizontalAlignment.Left, VerticalAlignment.Center);
        }

        private void DrawGraph(ICanvas canvas, RectF dirtyRect, int? maxCycles, int? highlightTouch, int? minRange, int? maxRange)
        {
            canvas.FillColor = Colors.DarkBlue;
            canvas.StrokeSize = 4;
            float canvasHeight = dirtyRect.Height - 100;
            float x_canvas = (dirtyRect.Width - (Vectors.Length * (rectangleWidth + rectangleSpacing))) / 2;

            int numTouch = maxCycles <= Vectors.Length ? maxCycles ?? 0 : Vectors.Length;

            // Loop through each rectangle
            for (int t = 0; t < numTouch; t++)
            {
                float x = t * (rectangleWidth + rectangleSpacing) + x_canvas;
                int maxCellValue = Vectors[t].Max() / 10;

                bool majorityLessThan500 = Vectors[t].Count(value => value < 500) > Vectors[t].Length / 2;

                float rectangleHeight = majorityLessThan500 ? 1 : 2;

                foreach (int cell in Vectors[t])
                {
                    if ((cell > minRange && cell < maxRange) || (minRange == null && maxRange == null) || (minRange == null && cell < maxRange) || (maxRange == null && cell > minRange))
                    {
                        float y = canvasHeight - (cell / 10);
                        canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                    }
                }

                canvas.Font = Font.DefaultBold;
                canvas.FontSize = 15;
                canvas.DrawString($" {t}", x, canvasHeight + 15, rectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Bottom);

                canvas.StrokeColor = Colors.Red;
                canvas.StrokeSize = 2;

                float y_height = canvasHeight - maxCellValue - 10;

                if (highlightTouch.HasValue)
                {
                    float x_highlight = (highlightTouch ?? 0) * (rectangleWidth + rectangleSpacing) + x_canvas;
                    canvas.DrawRoundedRectangle(x_highlight, y_height, rectangleWidth + 5, maxCellValue + 20, 5);
                }
            }
        }

        private void DrawTicks(ICanvas canvas, RectF dirtyRect)
        {
            float tickWidth = 10;
            float tickSpacing = 50;
            float tickStartX = (dirtyRect.Width - (Vectors.Length * (rectangleWidth + rectangleSpacing))) / 2 - tickWidth;

            float canvasHeight = dirtyRect.Height - 100;

            canvas.StrokeColor = Colors.Green;

            for (int t = 0; t < Vectors.Length; t++)
            {
                int maxCellValue = Vectors[t].Max() / 10;
                int numberOfTicks = (int)(maxCellValue / tickSpacing);

                for (int i = 0; i <= numberOfTicks; i++)
                {
                    float tickY = canvasHeight - (i * tickSpacing);
                    float tickValue = 10 * i * tickSpacing;

                    canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);

                    canvas.FontSize = 10;
                    canvas.FontColor = Colors.Black;
                    canvas.DrawString(tickValue.ToString(), tickStartX - 30, tickY - 5, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
                }
            }
        }
    }
}
