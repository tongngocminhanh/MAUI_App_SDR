using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;

namespace SDRrepresentation
{
    public class GraphicsDrawable : IDrawable
    {
        public int RoundedRadius { get; set; } = 10;
        // Define the data similar to the Python script
        /*private int[][] activeCellsColumn = new int[][]
        {
            new int[] {10, 20, 30},  // Sample data for active cells column
            new int[] {5, 15},        // Sample data for active cells column
            new int[] {25, 35, 45}    // Sample data for active cells column
        };

        private int highlightTouch = 1; // Sample value for the highlight*/

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Define the paint for drawing rectangle borders


            // Define the highlight color and paint for the red rectangle
            /* var highlightColor = new Color(1, 0, 0, 0.5f); // Semi-transparent red


             // Define the rectangle dimensions
             float rectangleWidth = 60;
             float rectangleHeight = 1; // Height of the rectangle
             float rectangleSpacing = 10; // Spacing between rectangles

             // Loop through each rectangle
             for (int t = 0; t < activeCellsColumn.Length; t++)
             {
                 // Calculate the x-coordinate for the rectangle
                 float x = t * (rectangleWidth + rectangleSpacing);

                 // Loop through each cell in the current rectangle
                 foreach (int cell in activeCellsColumn[t])
                 {
                     // Calculate the y-coordinate for the rectangle
                     float y = cell;

                     // Draw the rectangle
                     canvas.DrawRectangle(x, y, x + rectangleWidth, y + rectangleHeight); 



                     }
                 }
             }*/
            

        }
        private Rect DrawMeter(ICanvas canvas, RectF dirtyRect)
        {

            var x = dirtyRect.X;
            var y = dirtyRect.Y;
            var width = dirtyRect.Width;
            var height = dirtyRect.Height;


            var meterRect = new Rect(x, y, width, height);
            canvas.DrawRoundedRectangle(meterRect, (double)RoundedRadius);
            return meterRect;

        }
    }
}


