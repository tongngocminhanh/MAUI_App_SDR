using System.ComponentModel;
using Font = Microsoft.Maui.Graphics.Font;

namespace AppSDR.SdrDrawerLib
{
    /// <summary>
    /// Represents a drawable object that supports rendering graphical elements on a canvas.
    /// Implements <see cref="IDrawable"/> for drawing and <see cref="INotifyPropertyChanged"/> for property change notifications.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SdrDrawable"/> class with specified graph parameters.
        /// </summary>
        /// <param name="graphName">The name of the graph.</param>
        /// <param name="maxCycles">The maximum number of cycles, columns for the graph.</param>
        /// <param name="highlightTouch">The index for highlighting a specific column.</param>
        /// <param name="xAxisTitle">The title of the X-axis.</param>
        /// <param name="yAxisTitle">The title of the Y-axis.</param>
        /// <param name="minRange">The minimum range value for SDR values.</param>
        /// <param name="maxRange">The maximum range value for the SDR values.</param>
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

        /// <summary>
        /// Draws the inner border of the drawable area.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="dirtyRect">The rectangle defining the drawing area.</param>
        public void DrawInnerBorder(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.WhiteSmoke;
            canvas.FillRectangle(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
        }

        /// <summary>
        /// Draws the X-axis title with fitting alignment.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
        public void DrawXAxisFit(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Gray;
            float FontSize = 20;
            canvas.FontSize = FontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float TextWidth = XAxisTitle.Length * FontSize * 0.8f;
            float TextHeight = FontSize * 1.2f;

            float TextX = IRect.X + (IRect.Width - TextWidth) / 2;
            float TextY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

        /// <summary>
        /// Draws the X-axis title with extended alignment.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
        public void DrawXAxisExtend(ICanvas canvas, RectF IRect)
        {
            canvas.FontColor = Colors.Gray;
            float FontSize = 20;
            canvas.FontSize = FontSize;

            // Calculate text width and height using a rough estimate based on the font size
            float TextWidth = XAxisTitle.Length * FontSize * 0.8f;
            float TextHeight = FontSize * 1.2f;

            float TextX = 0;
            float TextY = IRect.Height - 30;

            canvas.DrawString($" {XAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

        /// <summary>
        /// Draws the Y-axis title.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
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

            canvas.Rotate(-90, TextX, TextY);
            canvas.DrawString($" {YAxisTitle}", TextX, TextY, TextWidth, TextHeight, HorizontalAlignment.Left, VerticalAlignment.Center);
            canvas.RestoreState();
        }

        /// <summary>
        /// Draws the graph name with fitting alignment.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
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

        /// <summary>
        /// Draws the graph name with extended alignment.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
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

        /// <summary>
        /// Draws a highlight rectangle based on the maximum cell value.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="IRect">The rectangle defining the drawing area.</param>
        /// <param name="maxCellValue">The maximum value to determine the highlight area.</param>
        public void DrawHighlight(ICanvas canvas, RectF IRect, int maxCellValue)
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;

            float y_highlight = IRect.Height - maxCellValue - 110;
            float x_highlight = ((HighlightTouch ?? 0)) * (RectangleWidth + RectangleSpacing) + XCanvas;

            canvas.DrawRoundedRectangle(x_highlight, y_highlight, RectangleWidth + 2, maxCellValue + 20, 5);
        }

        /// <summary>
        /// Draws column numbers on the canvas.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="dirtyRect">The rectangle defining the drawing area.</param>
        /// <param name="column">The column number to be drawn.</param>
        /// <param name="X">The X-coordinate for drawing the column number.</param>
        public void DrawColumnNumber(ICanvas canvas, RectF dirtyRect, int column, float X)
        {
            float canvasHeight = dirtyRect.Height - 90;

            canvas.Font = Font.DefaultBold;
            canvas.FontSize = 9;
            canvas.DrawString($" {column}", X, canvasHeight + 15, RectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }

        /// <summary>
        /// Draws tick marks and labels on the Y-axis.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="dirtyRect">The rectangle defining the drawing area.</param>
        /// <param name="maxCellValue">The maximum value to determine the number of tick marks.</param>
        /// <param name="tickWidth">The width of each tick mark.</param>
        /// <param name="tickSpacing">The spacing between each tick mark.</param>
        public void DrawTickMark(ICanvas canvas, RectF dirtyRect, int maxCellValue, float tickWidth, float tickSpacing)
        {
            float tickStartX = XCanvas - tickWidth;
            int numberOfTicks = (int)(maxCellValue / tickSpacing);
            float canvasHeight = dirtyRect.Height - 100;

            canvas.StrokeColor = Colors.Green;
            for (int i = 0; i <= numberOfTicks; i++)
            {
                float tickY = canvasHeight - (i * tickSpacing);
                float tickValue = 10 * i * tickSpacing;

                canvas.DrawLine(tickStartX, tickY, tickStartX + tickWidth, tickY);
                DrawTickLabel(canvas, tickValue, tickStartX, tickY);
            }
        }

        /// <summary>
        /// Draws a label next to each tick mark.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="tickValue">The value of the tick mark.</param>
        /// <param name="tickStartX">The X-coordinate for drawing the label.</param>
        /// <param name="tickY">The Y-coordinate for drawing the label.</param>
        public void DrawTickLabel(ICanvas canvas, float tickValue, float tickStartX, float tickY)
        {
            canvas.FontSize = 10;
            canvas.FontColor = Colors.Black;
            canvas.DrawString(tickValue.ToString(), tickStartX - 30, tickY - 5, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
        }

        /// <summary>
        /// Draws all elements of the drawable object on the canvas.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="dirtyRect">The rectangle defining the drawing area.</param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawInnerBorder(canvas, dirtyRect);
            DrawNameFit(canvas, dirtyRect);
            DrawXAxisExtend(canvas, IRect);
            DrawYAxis(canvas, IRect);
            DrawNameFit(canvas, IRect);
            DrawNameExtend(canvas, IRect);
        }
    }
}
