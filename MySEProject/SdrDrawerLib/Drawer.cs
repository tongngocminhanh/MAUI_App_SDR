using Microsoft.Maui.Graphics;

namespace SdrDrawerLib
{
    public class MyDrawable
    {
        public void DrawRectangle(ICanvas canvas, float x, float y, float width, float height)
        {
            var paint = new SolidPaint { Color = Colors.Blue };
            if (canvas == null)
            {
                // Handle null canvas object
                return;
            }

            if (paint == null)
            {
                // Handle null paint object
                return;
            }
            // Example: Draw a filled rectangle
            
            canvas.StrokeColor = paint.Color;
            canvas.DrawRectangle(x, y, height, width);
        }
    }
}
