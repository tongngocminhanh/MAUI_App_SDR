
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


    }
    // In Page1.xaml.cs or wherever the event handler is defined
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}
