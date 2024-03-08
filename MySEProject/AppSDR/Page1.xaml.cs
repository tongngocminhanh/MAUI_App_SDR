namespace AppSDR;
public partial class Page1 : ContentPage
{
    // Define a property to hold the text to be displayed
    public string[] EntryCellValues { get; set; }
    public Page1(int[][] activeCellsColumn, string[] entryCellValues)
    {
        InitializeComponent();

        // Assign entry cell values to the property
        EntryCellValues = entryCellValues;

        var graphicsView = this.DrawableView;
        var graphicsdraw = (GraphicsDraw)graphicsView.Drawable;
        graphicsdraw.Vectors = activeCellsColumn;
        graphicsdraw.GraphPara = EntryCellValues;

        //graphicsView.Invalidate();

        // Handle the SizeChanged event to redraw when the size changes
        graphicsView.SizeChanged += (sender, args) => DrawableView.Invalidate();

        // Set the BindingContext to the current page
        BindingContext = this;
    }
}
