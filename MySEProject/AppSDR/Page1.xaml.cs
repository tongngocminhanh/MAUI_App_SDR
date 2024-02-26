namespace AppSDR;

public partial class Page1 : ContentPage
{
    public readonly int[][] vectors;
    public Page1(int[][] activeCellsColumn)
    {
        InitializeComponent();

        vectors = activeCellsColumn;
        var drawable = new GraphicsDrawable();
        drawable.Vectors = vectors;
        Content = new GraphicsView
        {
            Drawable = drawable
        };


    }
}