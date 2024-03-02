
namespace AppSDR;

public partial class Page1 : ContentPage
{
    // Define a property to hold the text to be displayed
    public string[] EntryCellValues { get; set; }

    //public readonly int[][] vectors;
    public Page1(int[][] activeCellsColumn, string[] entryCellValues)
    {
        InitializeComponent();

        var graphicsView = this.DrawableView;
        var graphicsdrawable = (GraphicsDrawable)graphicsView.Drawable;
        graphicsdrawable.Vectors = activeCellsColumn;

        graphicsView.Invalidate();

        // Set the text to be displayed
        // Assign entry cell values to the property
        EntryCellValues = entryCellValues;

        // Set the BindingContext to the current page
        BindingContext = this;

    }
    // In Page1.xaml.cs or wherever the event handler is defined
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}
