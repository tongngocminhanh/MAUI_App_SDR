using Microsoft.Maui.Graphics;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR
{
    public class GraphicsDraw : BindableObject, IDrawable
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
            //int maxCycles = int.Parse(GraphPara[1]);
            //int highlightTouch = int.Parse(GraphPara[2]);
            string xAxisTitle = GraphPara[3];
            string yAxisTitle = GraphPara[4];
            //int maxRange = int.Parse(GraphPara[5]);
            //int minRange = int.Parse(GraphPara[6]);
            //string figureName = GraphPara[7];

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
            canvas.DrawString(yAxisTitle,(textWidth + borderHeightAdjusted)/2,textY, textWidth, textHeight, HorizontalAlignment.Left, VerticalAlignment.Top);

            // Restore the canvas state after drawing
            canvas.RestoreState();


            //canvas.FillColor = Colors.DarkBlue;
            //canvas.StrokeSize = 4;

            //canvas.FontColor = Colors.Black;
            //canvas.FontSize = 5;

            //// Calculate the dimensions based on the insideRect
            //float canvasWidth = insideRect.Width;
            //float canvasHeight = insideRect.Height;

            //// Define drawing spaces of each value
            //float rectangleWidth = (canvasWidth - (Vectors.Length * borderWidth * 2)) / (Vectors.Length + 1); // Adjust as needed
            //float rectangleSpacing = rectangleWidth / 2; // Adjust as needed
            //float tickWidth = 10; // Width of the tick marks
            //float tickSpacing = (canvasHeight - borderWidth * 2) / 10; // Spacing between tick marks
            //float tickStartX = rectangleSpacing + borderWidth; // Start X position for the ticks

            //// Loop through each rectangle
            //for (int t = 0; t < Vectors.Length; t++)
            //{
            //    float x = (t * rectangleWidth) + ((t + 1) * rectangleSpacing) + borderWidth;
            //    int maxCellValue = Vectors[t].Max() / 10;
            //    int numberOfTicks = (int)(maxCellValue / tickSpacing);

            //    // Draw rectangles and tick marks here...
            //    // Check if the majority value is less than 500
            //    bool majorityLessThan500 = Vectors[t].Count(value => value < 500) > Vectors[t].Length / 2;

            //    if (majorityLessThan500)
            //    {
            //        float rectangleHeight = 1;
            //        foreach (int cell in Vectors[t])
            //        {
            //            // Calculate the y-coordinate for the rectangle
            //            // Start from the bottom and decrement by rectangleHeight
            //            float y = insideRect.Height - (cell / 10) + borderWidth; // Adjust for the top border

            //            // Draw the rectangle
            //            canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
            //        }
            //    }
            //    else
            //    {
            //        float rectangleHeight = 2;
            //        foreach (int cell in Vectors[t])
            //        {
            //            // Calculate the y-coordinate for the rectangle
            //            // Start from the bottom and decrement by rectangleHeight
            //            float y = insideRect.Height - (cell / 10) + borderWidth; // Adjust for the top border

            //            // Draw the rectangle
            //            canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
            //        }
            //    }

            //    // Adjust the x-coordinate for drawing the string
            //    float stringX = x + (rectangleWidth / 2); // Center the string horizontally
            //    float stringY = insideRect.Height + borderWidth; // Align with bottom of insideRect

            //    canvas.Font = Font.DefaultBold;
            //    canvas.DrawString($" {t}", stringX, stringY, rectangleWidth, 30, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            //    // Loop through each cell in the current rectangle
            //    for (int i = 0; i <= numberOfTicks; i++)
            //    {
            //        // Calculate the y-coordinate for the current tick mark
            //        float tickY = insideRect.Height - (i * tickSpacing) + borderWidth; // Adjust for the top border

            //        // Draw the tick mark
            //        canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);

            //        // Draw the text label for the current tick mark
            //        float tickStringX = tickStartX - (tickWidth / 2); // Center the text with respect to the tick mark
            //        float tickStringY = tickY - 5; // Adjust for better alignment
            //        canvas.FontSize = 10;
            //        canvas.FontColor = Colors.Black;
            //        canvas.DrawString((10 * i).ToString(), tickStringX, tickStringY, tickWidth, 50, HorizontalAlignment.Center, VerticalAlignment.Top);
            //    }
            //}
        }
    }
}