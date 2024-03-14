
namespace AppSDR;

using AppSDR.ViewModel;
using System.ComponentModel;
public partial class Page1 : ContentPage, INotifyPropertyChanged
{
    public string[] EntryCellValues { get; set; }
    public Page1(int[][] activeCellsColumn, string[] entryCellValues)
    {
        InitializeComponent();
        EntryCellValues = entryCellValues;
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
      
    }
    private async void BackToMainPageButton_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PopModalAsync(); // Navigate back to the MainPage

    }

}



