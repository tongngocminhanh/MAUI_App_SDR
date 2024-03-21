using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR.SdrDrawerLib
{
    public class SdrDrawable : BindableObject, IDrawable, INotifyPropertyChanged
    {
        public RectF IRect { get; set; }
        public string GraphName { get; set; }
        public int? MaxCycles { get; set; }
        public int? HighlightTouch { get; set; }
        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public int? MinRange { get; set; }
        public int? MaxRange { get; set; }
        public int MaxCellValues { get; set; }
        public float RectangleWidth { get; set; }
        public float RectangleSpacing { get; set; }
        public float XCanvas { get; set; }
        // Constructor to initialize the properties
        public SdrDrawable(string graphName, int? maxCycles, int? highlightTouch, string xAxisTitle, string yAxisTitle, int? minRange, int? maxRange)
        {
            GraphName = graphName;
            MaxCycles = maxCycles;
            HighlightTouch = highlightTouch;
            XAxisTitle = xAxisTitle;
            YAxisTitle = yAxisTitle;
            MinRange = minRange;
            MaxRange = maxRange;

        }
        public void DrawInnerBorder(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.WhiteSmoke;
            canvas.FillRectangle(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
        }
        public void DrawXAxisFit(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Gray;
            float fontSize = 20;
            canvas.FontSize = fontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = XAxisTitle.Length * fontSize * 0.8f; // Roughly estimate text width based on the number of characters
            float textHeight = fontSize * 1.2f; // Roughly estimate text height based on font size

            float textX = IRect.X + (IRect.Width - textWidth) / 2;
            float textY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

        public void DrawXAxisExtend(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Gray;
            float fontSize = 20;
            canvas.FontSize = fontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = XAxisTitle.Length * fontSize * 0.8f; // Roughly estimate text width based on the number of characters
            float textHeight = fontSize * 1.2f; // Roughly estimate text height based on font size

            float textX = IRect.X;
            float textY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
        public void DrawYAxis(ICanvas canvas, RectF IRect)
        {
            canvas.SaveState();

            canvas.FontColor = Colors.Gray;
            float fontSize = 20;
            canvas.FontSize = fontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = XAxisTitle.Length * fontSize * 0.8f;
            float textHeight = fontSize * 1.2f;

            // Define x,y position of yAxisTitle
            float textX = IRect.X + 30;
            float textY = IRect.Height / 2;

            canvas.Rotate(-90, textX, textY); // Rotate the canvas by -90 degrees
            canvas.DrawString($" {YAxisTitle}", textX, textY, textWidth, textHeight, HorizontalAlignment.Left, VerticalAlignment.Center);
            canvas.RestoreState();
        }

        public void DrawNameFit(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Black;
            float fontSize = 20;
            canvas.FontSize = fontSize;
            canvas.Font = Font.DefaultBold;

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = GraphName.Length * fontSize * 0.8f;
            float textHeight = fontSize * 1.2f;

            // Define x,y position of GraphName
            float textX = IRect.X + (IRect.Width - textWidth) / 2;
            float textY = IRect.Y + 20;

            canvas.DrawString($" {GraphName}", textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
        public void DrawNameExtend(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Black;
            float fontSize = 20;
            canvas.FontSize = fontSize;
            canvas.Font = Font.DefaultBold;

            // Calculate text width and height using a rough estimate based on the font size
            float textWidth = GraphName.Length * fontSize * 0.8f;
            float textHeight = fontSize * 1.2f;

            // Define x,y position of GraphName
            float textX = IRect.X;
            float textY = IRect.Y + 20;

            canvas.DrawString($" {GraphName}", textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Top);
        }

        public void DrawHighlight(ICanvas canvas, RectF IRect, int? HightlightTouch, int MaxCellValues)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;

            // Define start x,y position of highlight
            float y_highlight = IRect.Height - MaxCellValues - 110;
            float x_highlight = ((HighlightTouch ?? 0)) * (RectangleWidth + RectangleSpacing) + XCanvas;

            canvas.DrawRoundedRectangle(x_highlight, y_highlight, RectangleWidth + 3, MaxCellValues + 20, 5);
        }

        public void DrawRectangles(ICanvas canvas, float x, float canvasHeight, int[] values, float rectangleWidth, int? minRange, int? maxRange)
        {
            // Determine rectangle height based on the majority condition
            float rectangleHeight = (values.Count(value => value < 500) > values.Length / 2) ? 1 : 2;

            foreach (int cell in values)
            {
                // Check if the cell value is within the specified range
                if ((cell > minRange && cell < maxRange) || (minRange == null && maxRange == null) ||
                    (minRange == null && cell < maxRange) || (maxRange == null && cell > minRange))
                {
                    float y = canvasHeight - (cell / 10);
                    canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);
                }
            }
        }

        public void DrawTickMarks(ICanvas canvas, float tickStartX, float canvasHeight, float tickSpacing, float tickWidth, int numberOfTicks)
        {
            canvas.StrokeColor = Colors.Green;
            canvas.Font = Font.DefaultBold;
            canvas.FontSize = 15;

            for (int i = 0; i <= numberOfTicks; i++)
            {
                // Calculate the y-coordinate for the current tick mark
                float tickY = canvasHeight - (i * tickSpacing);

                // Calculate the value associated with the current tick mark
                float tickValue = 10 * i * tickSpacing;

                // Draw the tick mark
                canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);

                // Draw the text label for the current tick mark
                canvas.FontSize = 10;
                canvas.FontColor = Colors.Black;
                canvas.DrawString(tickValue.ToString(), tickStartX - 30, tickY - 5, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawInnerBorder(canvas, dirtyRect);
            DrawNameFit(canvas, dirtyRect); 
            DrawXAxisExtend(canvas, IRect);
            DrawYAxis(canvas, IRect);
            DrawNameFit(canvas, IRect);
            DrawNameExtend(canvas, IRect);
            DrawHighlight(canvas, IRect, HighlightTouch, MaxCellValues);
        }
    }
}
