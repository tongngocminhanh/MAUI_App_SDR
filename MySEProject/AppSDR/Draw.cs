using Microsoft.Maui.Graphics;
using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR
{
    public class GraphicsDraw : BindableObject, IDrawable, INotifyPropertyChanged
    {
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
            // Check if GraphPara array has at least required number of elements
            if (GraphPara.Length < 2)
            {
                // Throw an exception or handle the error gracefully
                return;
            }

            // Access predefined data from GraphPara
            string graphName = GraphPara[0];
            int maxCycles = int.Parse(GraphPara[1]);
            int highlightTouch = int.Parse(GraphPara[2]);
            string xAxisTitle = GraphPara[3];
            string yAxisTitle = GraphPara[4];
            int maxRange = int.Parse(GraphPara[5]);
            int minRange = int.Parse(GraphPara[6]);

            // Define border properties
            float borderWidth = 30; // Adjust as needed
            float borderX = dirtyRect.X; // x-coordinate of top-left corner
            float borderY = dirtyRect.Y; // y-coordinate of top-left corner
            float borderWidthAdjusted = dirtyRect.Width; // initialize border width with width of drawing area
            float borderHeightAdjusted = dirtyRect.Height; // initialize border height with height of drawing area

            // Draw top, bottom, left, right border
            canvas.FillColor = Colors.White; // Adjust border color as needed
            canvas.FillRectangle(borderX, borderY, borderWidthAdjusted, borderWidth);
            canvas.FillRectangle(borderX, borderY + dirtyRect.Height - borderWidth*2, borderWidthAdjusted, borderWidth*2);
            canvas.FillRectangle(borderX, borderY, borderWidth*2, borderHeightAdjusted);
            canvas.FillRectangle(borderX + dirtyRect.Width - borderWidth, borderY, borderWidth, borderHeightAdjusted);


            // Adjust dirtyRect to fit inside the borders
            RectF insideRect = new RectF(borderX + borderWidth*2, borderY + borderWidth, borderWidthAdjusted - (3 * borderWidth), borderHeightAdjusted - (3 * borderWidth));

            // Now draw your content inside the borders
            canvas.FillColor = Colors.WhiteSmoke;
            // Draw the inside rectangle
            canvas.FillRectangle(insideRect.X, insideRect.Y, insideRect.Width, insideRect.Height);

            // Create paint for the text
            canvas.FillColor = Colors.Black;
            canvas.Font = Font.DefaultBold;
            // Define the font size for the text
            float fontSize = 40; // Adjust as needed

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = graphName.Length * fontSize * 0.8f; // Roughly estimate text width based on the number of characters
            float textHeight = fontSize * 1.2f; // Roughly estimate text height based on font size

            // Calculate text position within the canvas
            float textX = (borderWidthAdjusted - textWidth) / 2; // Center the text horizontally
            float textY = borderWidth / 2 - textHeight / 2; // Align the text with the top border

            // Draw the text on the canvas
            canvas.DrawString(graphName, textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Center);

            // Calculate text width and height using a rough estimate based on the font size
            textWidth = xAxisTitle.Length * fontSize * 0.8f; // Roughly estimate text width based on the number of characters
            textHeight = fontSize * 1.2f; // Roughly estimate text height based on font size

            // Calculate text position within the canvas
            textX = (borderWidthAdjusted - textWidth) / 2; // Align the text with the left border
            textY = insideRect.Bottom + textHeight/2; // Align the text with the bottom border

            // Draw the X-axis title on the canvas
            canvas.DrawString(xAxisTitle, textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Center);

            // Calculate text width and height using a rough estimate based on the font size
            textWidth = yAxisTitle.Length * fontSize * 0.8f; // Roughly estimate text width based on the number of characters
            textHeight = fontSize * 1.2f; // Roughly estimate text height based on font size

            // Calculate text position within the canvas
            textX = borderWidth - textHeight / 2; // Align the text with the left border
            textY = (borderHeightAdjusted - textWidth)/ 2; // Align the text with the center of the left border

            // Rotate the canvas for drawing the Y-axis title
            canvas.SaveState();
            canvas.Rotate(-90, textX, textY); // Rotate the canvas by -90 degrees
            // Draw the Y-axis title on the canvas
            canvas.DrawString(yAxisTitle, textX+5, textY -5, textWidth, textHeight, HorizontalAlignment.Left, VerticalAlignment.Top);

            // Restore the canvas state after drawing
            canvas.RestoreState();


            // Determine the range between maxRange and minRange
            int range = maxRange - minRange;

            // Calculate the interval for gridlines and labels
            int interval = range / 10; // Divide the range into 10 intervals

            // Calculate the spacing between gridlines
            float spacing = insideRect.Height / 10; // Divide the height into 10 equal parts
                                                    // Draw the gridlines and labels

            List<int> gridlines = new List<int>();// division
            for (int i = 0; i <= 10; i++)
            {
                // Calculate the y-coordinate for the gridline
                float gridY = insideRect.Bottom - (spacing * i);

                // Draw the gridline
                canvas.StrokeColor = Colors.LightGray; // Adjust color as needed
                canvas.DrawLine(borderWidth * 5 / 3, gridY, borderWidth * 2, gridY);

                // Calculate the value for the label
                int labelValue = minRange + interval * i;
                gridlines.Add(labelValue);

                // Draw the label
                string labelText = labelValue.ToString();
                float labelWidth = labelText.Length * fontSize * 0.5f;
                float labelX = borderWidth * 5 / 3 - labelWidth;
                float labelY = gridY - fontSize / 2; // Adjust for centering the label vertically
                canvas.DrawString(labelText, labelX, labelY, labelWidth, fontSize, HorizontalAlignment.Right, VerticalAlignment.Center);
            }

            // Draw columns
            // Calculate the width of each column
            float columnWidth = (borderWidthAdjusted - (3 * borderWidth)) / maxCycles;

            // Add space between columns
            float spaceBetweenColumns = 5; // Adjust as needed

            // Calculate the total width required for drawing all columns
            float totalColumnWidth = (columnWidth + spaceBetweenColumns) * maxCycles;

            // Calculate the starting x-coordinate for drawing columns within the borders
            float columnStartX = borderWidth * 2; // Start drawing columns after the left border

            // Calculate the width of each column
            columnWidth = Math.Min(columnWidth, (insideRect.Width / maxCycles) - spaceBetweenColumns); // Adjust column width if it exceeds available width

            // Define drawing spaces of each value
            float rectangleWidth = (insideRect.Width - (Vectors.Length * borderWidth * 2)) / (Vectors.Length + 1); // Adjust as needed
            float rectangleSpacing = rectangleWidth / 2; // Adjust as needed
            float tickWidth = 10; // Width of the tick marks
            float tickSpacing = (insideRect.Height - borderWidth * 2) / 10; // Spacing between tick marks
            float tickStartX = rectangleSpacing + borderWidth; // Start X position for the ticks

            // Loop through each rectangle
            for (int t = 0; t < Vectors.Length; t++)
            {
                float x = (t * rectangleWidth) + ((t + 1) * rectangleSpacing) + borderWidth;
                int maxCellValue = Vectors[t].Max() / 10;
                int numberOfTicks = (int)(maxCellValue / tickSpacing);

                // Draw rectangles and tick marks here...
                // Check if the majority value is less than 500
                bool majorityLessThan500 = Vectors[t].Count(value => value < 500) > Vectors[t].Length / 2;

                if (majorityLessThan500)
                {
                    float rectangleHeight = 1;
                    foreach (int cell in Vectors[t])
                    {
                        // Calculate the y-coordinate for the rectangle
                        // Start from the bottom and decrement by rectangleHeight
                        float y = insideRect.Height - (cell / 10) + borderWidth; // Adjust for the top border

                        // Draw the rectangle
                        canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                    }
                }
                else
                {
                    float rectangleHeight = 2;
                    foreach (int cell in Vectors[t])
                    {
                        // Calculate the y-coordinate for the rectangle
                        // Start from the bottom and decrement by rectangleHeight
                        float y = insideRect.Height - (cell / 10) + borderWidth; // Adjust for the top border

                        // Draw the rectangle
                        canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                    }
                }


                //// Loop through each cell in the current rectangle
                //for (int i = 0; i <= numberOfTicks; i++)
                //{
                //    // Calculate the y-coordinate for the current tick mark
                //    float tickY = insideRect.Height - (i * tickSpacing) + borderWidth; // Adjust for the top border

                //    // Draw the tick mark
                //    canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);

                //    // Draw the text label for the current tick mark
                //    float tickStringX = tickStartX - (tickWidth / 2); // Center the text with respect to the tick mark
                //    float tickStringY = tickY - 5; // Adjust for better alignment
                //    canvas.FontSize = 10;
                //    canvas.FontColor = Colors.Black;
                //    canvas.DrawString((10 * i).ToString(), tickStringX, tickStringY, tickWidth, 50, HorizontalAlignment.Center, VerticalAlignment.Top);
                //}
            }

            for (int i = 0; i < maxCycles; i++)
            {
                // Calculate the x-coordinate of the column
                float columnX = columnStartX + (columnWidth + spaceBetweenColumns) * i;
                // Set color based on whether the index i is even or odd
                if (i % 2 == 0)
                    canvas.FillColor = Colors.Blue;
                else
                    canvas.FillColor = Colors.Yellow;

                // Draw each column
                canvas.FillRectangle(columnX, insideRect.Bottom, columnWidth, - insideRect.Height);
            }

            // Define the highlight
            float rectX = columnStartX + columnWidth*highlightTouch + spaceBetweenColumns*(highlightTouch-1); // X-coordinate of the top-left corner of the rectangle
            float lineWidth = 5; 
            float rectWidth = columnWidth + spaceBetweenColumns*2; // Width of the rectangle
            float rectHeight = insideRect.Height+lineWidth*2; // Height of the rectangle


            // Set the color and line width for drawing the rectangle
            canvas.StrokeColor = Colors.Red; // Color of the rectangle border line
            canvas.StrokeSize = lineWidth; // Width of the rectangle border line

            // Draw the rectangle border line
            canvas.DrawRectangle(rectX, insideRect.Top-lineWidth, rectWidth, rectHeight);
        }
    }
}