using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR
{
   
    public class GraphicsDrawable : BindableObject, IDrawable
    {
        public int[][] Vectors
        {
            get => (int[][])GetValue(VectorsProperty);
            set => SetValue(VectorsProperty, value);

        }


        public static BindableProperty VectorsProperty = BindableProperty.Create(nameof(Vectors), typeof(int[][]), typeof(GraphicsDrawable));
        

       

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.DarkBlue;
            canvas.StrokeSize = 4;

            canvas.FontColor = Colors.Blue;
            canvas.FontSize = 10;


            float rectangleWidth = 25;
            float rectangleHeight = 2;
            float rectangleSpacing = 10;

            // Calculate the total height of the canvas
            float canvasHeight = dirtyRect.Height - 100;

            // Start drawing from the bottom of the canvas

            // Loop through each rectangle
            for (int t = 0; t < Vectors.Length; t++)
            {
                float x = t * (rectangleWidth + rectangleSpacing);

                // Loop through each cell in the current rectangle
                foreach (int cell in Vectors[t])
                {
                    // Calculate the y-coordinate for the rectangle
                    // Start from the bottom and decrement by rectangleHeight
                    float y = canvasHeight - (cell / 10);

                    // Draw the rectangle
                    canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);


                }
                canvas.Font = Font.DefaultBold;
                canvas.DrawString($"SDR {t}", x, canvasHeight, rectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }
    }
}