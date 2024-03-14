using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR
{

    public class GraphicsDrawable : BindableObject, IDrawable, INotifyPropertyChanged
    {
        public int[][] Vectors
        {
            get => (int[][])GetValue(VectorsProperty);
            set => SetValue(VectorsProperty, value);

        }


        public static BindableProperty VectorsProperty = BindableProperty.Create(nameof(Vectors), typeof(int[][]), typeof(GraphicsDrawable));

        public string[] GraphPara
        {
            get => (string[])GetValue(GraphParaProperty);
            //get { return (string[])GetValue(GraphParaProperty); }
            set => SetValue(GraphParaProperty, value);
        }

        public static readonly BindableProperty GraphParaProperty = BindableProperty.Create(nameof(GraphPara), typeof(string[]), typeof(GraphicsDrawable));

        // Access predefined data from GraphPara


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (GraphPara != null && GraphPara.Length >= 7)     // Ensure GraphPara is not null and has at least 7 elements
            {
                //string[] entryCellValues = GraphPara;
                //string graphName = entryCellValues[0];
                ////int maxCycles = int.Parse(GraphPara[1]);
                //int highlightTouch = int.Parse(entryCellValues[2]);
                //string xAxisTitle = entryCellValues[3];
                //string yAxisTitle = entryCellValues[4];
                //int minRange = int.Parse(entryCellValues[5]);
                //int maxRange = int.Parse(entryCellValues[6]);
                // Access predefined data from GraphPara
                string graphName = GraphPara[0];
                //int maxCycles = int.Parse(GraphPara[1]);
                int highlightTouch = int.Parse(GraphPara[2]);
                string xAxisTitle = GraphPara[3];
                string yAxisTitle = GraphPara[4];
                int minRange = int.Parse(GraphPara[5]);
                int maxRange = int.Parse(GraphPara[6]);


                canvas.FillColor = Colors.WhiteSmoke;
                canvas.FillRectangle(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);


                canvas.FillColor = Colors.DarkBlue;
                canvas.StrokeSize = 4;

                canvas.FontColor = Colors.Black;
                canvas.FontSize = 15;


                float rectangleWidth = 25;

                float rectangleSpacing = 10;


                float canvasHeight = dirtyRect.Height - 100;
                float canvasWidth = dirtyRect.Width;


                //// Calculate the horizontal offset to center the drawing
                float x_canvas = (canvasWidth - (Vectors.Length * (rectangleWidth + rectangleSpacing))) / 2;
                canvas.DrawString($" {graphName}", x_canvas, 70, 200, 50, HorizontalAlignment.Center, VerticalAlignment.Top);
                canvas.DrawString($" {xAxisTitle}", x_canvas, canvasHeight, 200, 70, HorizontalAlignment.Center, VerticalAlignment.Bottom);
                canvas.DrawString($" {yAxisTitle}", x_canvas - 160, canvasHeight - 200, 200, 70, HorizontalAlignment.Left, VerticalAlignment.Center);




                //// Start drawing from the bottom of the canvas
                //// Draw tick marks on the left side
                float tickWidth = 10; // Width of the tick marks
                float tickSpacing = 100; // Spacing between tick marks
                float tickStartX = x_canvas - tickWidth; // X-coordinate of the tick marks



                // Loop through each rectangle
                for (int t = 0; t < Vectors.Length; t++)
                {
                    float x = t * (rectangleWidth + rectangleSpacing) + x_canvas;
                    int maxCellValue = Vectors[t].Max() / 10;

                    int numberOfTicks = (int)(maxCellValue / tickSpacing);

                    // Check if the majority value is less than 500
                    bool majorityLessThan500 = Vectors[t].Count(value => value < 500) > Vectors[t].Length / 2;

                    if (majorityLessThan500)
                    {
                        float rectangleHeight = 1;
                        foreach (int cell in Vectors[t])
                        {
                            if (cell <= maxRange && cell >= minRange)
                            {
                                // Calculate the y-coordinate for the rectangle
                                // Start from the bottom and decrement by rectangleHeight
                                float y = canvasHeight - (cell / 10);

                                // Draw the rectangle
                                canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                            }

                        }

                    }
                    else
                    {
                        float rectangleHeight = 2;
                        foreach (int cell in Vectors[t])
                        {
                            if (cell <= maxRange && cell >= minRange)
                            {
                                float y = canvasHeight - (cell / 10);

                                // Draw the rectangle
                                canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                            }
                            // Calculate the y-coordinate for the rectangle
                            // Start from the bottom and decrement by rectangleHeight



                        }


                    }
                    canvas.Font = Font.DefaultBold;
                    canvas.DrawString($" {t}", x, canvasHeight, rectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Bottom);

                    canvas.StrokeColor = Colors.Red;
                    canvas.StrokeSize = 4;
                    float x_highlight = highlightTouch * (rectangleWidth + rectangleSpacing) + x_canvas;
                    float y_height = canvasHeight - maxCellValue - 10;
                    canvas.DrawRoundedRectangle(x_highlight, y_height, rectangleWidth + 5, maxCellValue + 5, 5);







                    // Loop through each cell in the current rectangle
                    canvas.StrokeColor = Colors.Green;
                    for (int i = 0; i <= numberOfTicks; i++)
                    {
                        // Calculate the y-coordinate for the current tick mark
                        float tickY = canvasHeight - (i * tickSpacing);

                        // Calculate the value associated with the current tick mark
                        float tickValue = 10 * i * tickSpacing; // Adjusted calculation to start from 0

                        // Draw the tick mark
                        canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);

                        // Draw the text label for the current tick mark
                        canvas.FontSize = 10;
                        canvas.FontColor = Colors.Black;
                        canvas.DrawString(tickValue.ToString(), tickStartX - 30, tickY - 5, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
                    }





                }

            }
        }
    }
}