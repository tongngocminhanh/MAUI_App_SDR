using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;
using AppSDR.SdrDrawerLib;

namespace AppSDR.ViewModel
{
    public class Page1ViewModel : BindableObject, IDrawable, INotifyPropertyChanged
    {
        int numTouch;
        public int[][] Vectors
        {
            get => (int[][])GetValue(VectorsProperty);
            set => SetValue(VectorsProperty, value);
        }

        public static BindableProperty VectorsProperty = BindableProperty.Create(nameof(Vectors), typeof(int[][]), typeof(Page1ViewModel));

        public string[] GraphPara
        {
            get => (string[])GetValue(GraphParaProperty);
            set => SetValue(GraphParaProperty, value);
        }

        public static readonly BindableProperty GraphParaProperty = BindableProperty.Create(nameof(GraphPara), typeof(string[]), typeof(Page1ViewModel));

        // Access predefined data from GraphPara
        public float rectangleWidth { get; set; }
        public float rectangleSpacing { get; set; }
        public float widthRequest { get; set; }
        public float heightRequest { get; set; }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Check if all needed 7 parameters are assigned
            if (GraphPara != null && GraphPara.Length >= 7)
            {
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

                // Define drawing area
                float left = dirtyRect.Width/2 - widthRequest / 2;
                RectF rectangle = new RectF(left, 0, widthRequest, heightRequest);

                // Assign drawing area to SdrDrawerLib
                SdrDrawable drawable = new SdrDrawable(graphName,maxCycles,highlightTouch,xAxisTitle,yAxisTitle,minRange,maxRange);
                drawable.DrawInnerBorder(canvas, dirtyRect);

                // Draw the axis titles and graph name
                drawable.IRect = rectangle;
                drawable.DrawYAxis(canvas, rectangle);

                // Condition for axises position for better view
                if ((widthRequest < 35000) && (widthRequest > 1250))
                {
                    drawable.DrawXAxisExtend(canvas, rectangle);
                    drawable.DrawNameExtend(canvas, rectangle);
                }
                else
                {
                    drawable.DrawXAxisFit(canvas, rectangle);
                    drawable.DrawNameFit(canvas, rectangle);
                }

                // Start drawing from the bottom of the canvas
                float canvasHeight = dirtyRect.Height - 100;
                float canvasWidth = dirtyRect.Width;

                // Calculate the horizontal offset to center the drawing
                float x_canvas = (canvasWidth - (Vectors.Length * (rectangleWidth + rectangleSpacing))) / 2;

                // Draw tick marks on the left side
                float tickWidth = 10; // Width of the tick marks
                float tickSpacing = 50; // Spacing between tick marks
                float tickStartX = x_canvas - tickWidth; // X-coordinate of the tick marks

                if (maxCycles <= Vectors.Length)
                {
                    numTouch = maxCycles ?? 0;
                }
                else
                {
                    numTouch = Vectors.Length;
                }

                // Loop through each rectangle
                canvas.FillColor = Colors.DarkBlue;
                canvas.StrokeSize = 4;
                for (int t = 0; t < numTouch; t++)
                {
                    float x = t * (rectangleWidth + rectangleSpacing) + x_canvas;
                    int maxCellValue = Vectors[t].Max() / 10;

                    int numberOfTicks = (int)(maxCellValue / tickSpacing);

                    drawable.MaxCellValues = maxCellValue;
                    drawable.RectangleWidth = rectangleWidth;
                    drawable.RectangleSpacing = rectangleSpacing;
                    drawable.XCanvas = x_canvas;

                    // Draw highlight of the wanted column
                    if (highlightTouch.HasValue)
                    {
                        if (highlightTouch == t)
                        {
                            drawable.DrawHighlight(canvas, rectangle, highlightTouch, maxCellValue);
                        }
                    }

                    // Check if the majority value is less than 500
                    bool majorityLessThan500 = Vectors[t].Count(value => value < 500) > Vectors[t].Length / 2;

                    if (majorityLessThan500)
                    {
                        float rectangleHeight = 1;
                        foreach (int cell in Vectors[t])
                        {
                            // Condition of min, max value range defined by users
                            if ((cell > minRange && cell < maxRange) || (minRange == null && maxRange == null) || 
                                (minRange == null && cell < maxRange) || (maxRange == null && cell > minRange))

                            {
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
                            if ((cell > minRange && cell < maxRange) || (minRange == null && maxRange == null) || 
                                (minRange == null && cell < maxRange) || (maxRange == null && cell > minRange))
                            {
                                float y = canvasHeight - (cell / 10);

                                // Draw the rectangle
                                canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                            }
                        }
                    }

                    canvas.Font = Font.DefaultBold;
                    canvas.FontSize = 15;
                    canvas.DrawString($" {t}", x, canvasHeight + 15, rectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Bottom);

                    // Loop through each cell in the current rectangle
                    canvas.StrokeColor = Colors.Green;
                    for (int i = 0; i <= numberOfTicks; i++)
                    {
                        // Calculate the y-coordinate for the current tick mark
                        float tickY = canvasHeight - (i * tickSpacing);

                        // Calculate the value associated with the current tick mark
                        // Adjusted calculation to start from 0
                        float tickValue = 10 * i * tickSpacing; 

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
