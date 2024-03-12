
namespace AppSDR;

public partial class Page1 : ContentPage
{
    public string[] EntryCellValues { get; set; }

    //public readonly int[][] vectors;
    public Page1(int[][] activeCellsColumn, string[] entryCellValues)
    {
        InitializeComponent();

        var graphicsView = this.DrawableView;
        var graphicsdrawable = (GraphicsDrawable)graphicsView.Drawable;
        graphicsdrawable.Vectors = activeCellsColumn;
        graphicsdrawable.GraphPara = entryCellValues;
        int maxCellValue = 0;
        foreach (var column in activeCellsColumn)
        {
            foreach (var value in column)
            {
                if (value > maxCellValue)
                {
                    maxCellValue = value;
                }
            }
        }
        graphicsView.HeightRequest = maxCellValue/10 + 200;
        graphicsView.Invalidate();
      

        // Set the BindingContext to the current page
        BindingContext = this;

        
    }
    //private async void BackButton_Clicked(object sender, EventArgs e)
    //{
    //    await Navigation.PopAsync();
    //}

    // Other code in Page1 class
}
