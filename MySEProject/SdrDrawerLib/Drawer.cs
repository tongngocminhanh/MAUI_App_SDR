using Microsoft.Maui.Graphics;

namespace SdrDrawerLib
{
    public class Drawer
    {
        public void DrawRectangle(ICanvas canvas, float x, float y, float width, float height)
        {
            var color = Colors.Blue;
            var paint = new SolidPaint(color);
            canvas.StrokeColor = paint.Color;
            canvas.FillRectangle(x, y, width, height);
        }
    }
}
