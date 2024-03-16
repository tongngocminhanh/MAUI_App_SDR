using AppSDR.ViewModel;
using System.ComponentModel;

namespace AppSDR;
public partial class Page1 : ContentPage, INotifyPropertyChanged
{
    public string[] EntryCellValues { get; set; }
    public Page1(int[][] activeCellsColumn, string[] entryCellValues)
    {
        InitializeComponent();

        // Define source of drawing method
        var graphicsView = this.DrawableView;
        var graphicsdrawable = (Page1ViewModel)graphicsView.Drawable;

        // Parse a list of parameters and a matrix of SDR values
        EntryCellValues = entryCellValues;
        graphicsdrawable.Vectors = activeCellsColumn;
        graphicsdrawable.GraphPara = entryCellValues;

        // Define vertical screen size based on maximum cell value
        // Take the maximum value to set upper limit of the height
        float Rectwidth;
        float Rectspacing;
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

        graphicsView.HeightRequest = maxCellValue / 10 + 200;

        // Define horizontal screen size based on total SDR columns
        int max_xvalue = activeCellsColumn.Length;
        int max_widthvalue = activeCellsColumn.Length * 7 + 200;

        if (max_xvalue < 31)
        {
            Rectwidth = 20;
            Rectspacing = 10;
            graphicsView.WidthRequest = 350 + max_xvalue * (Rectwidth + Rectspacing);
        }
        else
        {
            Rectwidth = 5;
            Rectspacing = 2;
            if (max_widthvalue < 12000)
            {
                graphicsView.WidthRequest = max_widthvalue;
            }
            else
            {
                DisplayAlert("File Index Alert", "The file index exceeds the width of the view.", "OK");
            }

        }

        graphicsdrawable.rectangleSpacing = Rectspacing;
        graphicsdrawable.rectangleWidth = Rectwidth;
        graphicsView.Invalidate();
    }

    private async void BackToMainPageButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(); // Navigate back to the MainPage
    }
}