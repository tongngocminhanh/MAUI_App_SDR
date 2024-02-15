using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;
using Font = Microsoft.Maui.Graphics.Font;

namespace SDRrepresentation
{
    public class GraphicsDrawable : IDrawable
    {
        public int RoundedRadius { get; set; } = 10;

        private int[][] activeCellsColumn = new int[][]
        {
            new int[] {33, 34, 35, 36, 37, 39, 96, 97, 98, 99, 100, 103, 104, 167, 231, 232, 295, 359, 360, 423, 424, 1078, 1141, 1185, 1191, 1192, 1249, 1255, 1312, 1313, 1320, 1373, 1375, 1376, 1377, 1379, 1381, 1382, 1383, 1384, 1436, 1437, 1438, 1439, 1440, 1441, 1442, 1443, 1444, 1446, 1447, 1505, 1508, 1569, 1571, 1574, 1575, 1576, 1632, 1633, 1634, 1635, 1636, 1639, 1640, 1698, 1699, 1700, 1703, 2848, 2912, 3111, 4000, 4005, 4006, 4007, 4008, 4066, 4067, 4069, 4070},  // Sample data for active cells column
            new int[] {1569, 1760, 1762, 1767, 1826, 1832, 1895, 1956, 2018, 2023, 2084, 2087, 2144, 2146, 2147, 2151, 2152, 2272, 2274, 2276, 2400, 2401, 2464, 2467, 2468, 2471, 2530, 2532, 2599, 2600, 2658, 2659, 2660, 2663, 2720, 2721, 2728, 2786, 2787, 2850, 2920, 2977, 2979, 3044, 3106, 3107, 3234, 3240, 3296, 3299, 3300, 3303, 3361, 3364, 3427, 3428, 3488, 3489, 3491, 3496, 3552, 3554, 3555, 3616, 3618, 3619, 3681, 3682, 3684, 3688, 3747, 3748, 3811, 3812, 3874, 3879, 3880, 3937, 3940, 3944, 4000},        // Sample data for active cells column
            new int[] { 791, 860, 919, 924, 925, 984, 1046, 1110, 1115, 1116, 1175, 1239, 1243, 1303, 1365, 1366, 1367, 1371, 1495, 1557, 1560, 1564, 1565, 1687, 1749, 1750, 1751, 1752, 1756, 1821, 1883, 1943, 1949, 2005, 2011, 2013, 2077, 2139, 2197, 2261, 2326, 2332, 2389, 2453, 2456, 2517, 2587, 2651, 2711, 2776, 2909, 2966, 2967, 3094, 3100, 3159, 3164, 3288, 3351, 3352, 3413, 3419, 3420, 3477, 3479, 3483, 3544, 3671, 3797, 3799, 3861, 3862, 3869, 3925, 3926, 3928, 3932, 3990, 3991, 3996, 4056 },    // Sample data for active cells column
            new int[] { 214, 221, 279, 344, 470, 477, 599, 664, 667, 727, 790, 791, 919, 924, 925, 1046, 1110, 1175, 1239, 1242, 1303, 1365, 1366, 1367, 1495, 1557, 1560, 1564, 1565, 1625, 1655, 1657, 1658, 1687, 1749, 1752, 1818, 1821, 1883, 1914, 1943, 1977, 2005, 2073, 2074, 2077, 2197, 2234, 2261, 2298, 2326, 2332, 2386, 2389, 2453, 2456, 2458, 2517, 2555, 2587, 2771, 2772, 2966, 2967, 3094, 3159, 3164, 3167, 3418, 3420, 3483, 3544, 3671, 3861, 3862, 3869, 3932, 3991, 3996, 3998, 4056 },
            new int[] { 1057, 1061, 1062, 1182, 1185, 1189, 1249, 1254, 1311, 1312, 1441, 1503, 1566, 1573, 1630, 1694, 1697, 1760, 1822, 1830, 1893, 2014, 2017, 2078, 2081, 2143, 2144, 2149, 2150, 2273, 2336, 2400, 2401, 2405, 2463, 2470, 2528, 2534, 2597, 2655, 2718, 2719, 2847, 2854, 2910, 2911, 2917, 2918, 2974, 2975, 3041, 3166, 3167, 3237, 3238, 3295, 3302, 3360, 3361, 3429, 3487, 3488, 3489, 3550, 3557, 3616, 3622, 3679, 3681, 3686, 3742, 3743, 3745, 3750, 3806, 3807, 3808, 3936, 3937, 4062, 4069 },

        };

        private int highlightTouch = 1; // Sample value for the highlight

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.DarkBlue;
            canvas.StrokeSize = 4;

            canvas.FontColor = Colors.Blue;
            canvas.FontSize = 10;
           

            float rectangleWidth = 25;
            float rectangleHeight = 2;
            float rectangleSpacing = 10;

            // Calculate the total height of the canvas
            float canvasHeight = dirtyRect.Height-100;

            // Start drawing from the bottom of the canvas

            // Loop through each rectangle
            for (int t = 0; t < activeCellsColumn.Length; t++)
            {
                float x = t * (rectangleWidth + rectangleSpacing);

                // Loop through each cell in the current rectangle
                foreach (int cell in activeCellsColumn[t])
                {
                    // Calculate the y-coordinate for the rectangle
                    // Start from the bottom and decrement by rectangleHeight
                    float y = canvasHeight - (cell / 10);

                    // Draw the rectangle
                    canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);


                }
                canvas.Font = Font.DefaultBold;
                canvas.DrawString($"SDR {t}", x, canvasHeight, rectangleWidth, 30, HorizontalAlignment.Left, VerticalAlignment.Top);
            }
        }

        /*public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.DarkBlue;
            canvas.StrokeSize = 4;





            // Define the highlight color and paint for the red rectangle
            //var highlightColor = new Color(1, 0, 0, 0.5f); // Semi-transparent red


            // Define the rectangle dimensions
            float rectangleWidth = 25;
            float rectangleHeight = 2; // Height of the rectangle
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
                    float y = cell / 10;

                    // Draw the rectangle
                    canvas.FillRectangle(x, y, rectangleWidth, rectangleHeight);



                }
            }

        }*/
    }
}


    
       
    



