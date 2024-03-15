
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
        graphicsView.HeightRequest = maxCellValue/10 + 200;
    
        int max_xvalue = activeCellsColumn.Length * 25 + 10;
        int max_widthvalue= activeCellsColumn.Length * 7 + 200;

        if (max_xvalue < 1000)
        {
            Rectwidth = 25;
            Rectspacing = 10;
            graphicsView.WidthRequest = 1000;
        }
        else
        {
            Rectwidth = 5;
            Rectspacing = 2;
            if (max_widthvalue<12000)
            {
                graphicsView.WidthRequest = max_widthvalue;
            }
            else
            {
                DisplayAlert("File Index Alert", "The file index exceeds the width of the view.", "OK");
            }

        }

       
        graphicsdrawable.rectangleSpacing = Rectspacing;
        graphicsdrawable.rectangleWidth=Rectwidth;
        graphicsView.Invalidate();
      

    }
  
    private async void BackToMainPageButton_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PopModalAsync(); // Navigate back to the MainPage

    }

}



