
namespace AppSDR;

public partial class Page1 : ContentPage
{

    //public readonly int[][] vectors;
    public Page1(int[][] activeCellsColumn)
    {
        InitializeComponent();

        var graphicsView = this.DrawableView;
        var graphicsdrawable = (GraphicsDrawable)graphicsView.Drawable;
        graphicsdrawable.Vectors = activeCellsColumn;

        graphicsView.Invalidate();


        /*var drawable = new GraphicsDrawable();
            drawable.Vectors = vectors;
            Content = new GraphicsView
            {
                Drawable = drawable
            };*/


    }

    // Other code in Page1 class
}
