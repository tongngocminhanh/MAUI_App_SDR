using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;

namespace MauiApp1.SdrDrawerLib
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
            float FontSize = 20;
            canvas.FontSize = FontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float TextWidth = XAxisTitle.Length * FontSize * 0.8f; // Roughly estimate text width based on the number of characters
            float TextHeight = FontSize * 1.2f; // Roughly estimate text height based on font size

            float TextX = IRect.X + (IRect.Width - TextWidth) / 2;
            float TextY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
        public void DrawXAxisExtend(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Gray;
            float FontSize = 20;
            canvas.FontSize = FontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float TextWidth = XAxisTitle.Length * FontSize * 0.8f; // Roughly estimate text width based on the number of characters
            float TextHeight = FontSize * 1.2f; // Roughly estimate text height based on font size

            float TextX = 0;
            float TextY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }
        public void DrawYAxis(ICanvas canvas, RectF IRect)
        {
            canvas.SaveState();

            canvas.FontColor = Colors.Gray;
            float FontSize = 20;
            canvas.FontSize = FontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float TextWidth = XAxisTitle.Length * FontSize * 0.8f;
            float TextHeight = FontSize * 1.2f;

            // Define x,y position of yAxisTitle
            float TextX = IRect.X + 20;
            float TextY = IRect.Height / 2;

            canvas.Rotate(-90, TextX, TextY); // Rotate the canvas by -90 degrees
            canvas.DrawString($" {YAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Left, VerticalAlignment.Center);
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
            float textX = textWidth;
            float textY = IRect.Y + 20;

            canvas.DrawString($" {GraphName}", textX, textY, textWidth, textHeight, HorizontalAlignment.Center, VerticalAlignment.Top);
        }
        public void DrawHighlight(ICanvas canvas, RectF IRect, int maxCellValue)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;

            // Define start x,y position of highlight
            float y_highlight = IRect.Height - maxCellValue - 110;
            float x_highlight = ((HighlightTouch ?? 0)) * (RectangleWidth + RectangleSpacing) + XCanvas;

            canvas.DrawRoundedRectangle(x_highlight, y_highlight, RectangleWidth + 2, maxCellValue + 20, 5);
        }
        public void DrawColumnNumber(ICanvas canvas, RectF dirtyRect, int column, float X)
        {
            // Start drawing from the bottom of the canvas
            float canvasHeight = dirtyRect.Height - 90;

            canvas.Font = Font.DefaultBold;
            canvas.FontSize = 9;
            canvas.DrawString($" {column}", X, canvasHeight + 15, RectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }
        public void DrawTickMark(ICanvas canvas, RectF dirtyRect, int maxCellValue, float tickWidth, float tickSpacing)
        {
            // Draw tick marks on the left side
            float tickStartX = XCanvas - tickWidth; // X-coordinate of the tick marks
            int numberOfTicks = (int)(maxCellValue / tickSpacing);
            float canvasHeight = dirtyRect.Height - 100;

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
                DrawTickLabel(canvas, tickValue, tickStartX, tickY);
            }
        }
        public void DrawTickLabel(ICanvas canvas, float tickValue, float tickStartX, float tickY)
        {
            // Draw the text label for the current tick mark
            canvas.FontSize = 10;
            canvas.FontColor = Colors.Black;
            canvas.DrawString(tickValue.ToString(), tickStartX - 30, tickY - 5, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Basic drawing function when called Draw
            DrawInnerBorder(canvas, dirtyRect);
            DrawNameFit(canvas, dirtyRect); 
            DrawXAxisExtend(canvas, IRect);
            DrawYAxis(canvas, IRect);
            DrawNameFit(canvas, IRect);
            DrawNameExtend(canvas, IRect);
        }
    }
}
