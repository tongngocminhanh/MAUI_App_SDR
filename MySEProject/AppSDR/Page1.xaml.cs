
namespace AppSDR;
using AppSDR.ViewModel;
using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui;


public partial class Page1 : ContentPage, INotifyPropertyChanged
{
 
    public string[] EntryCellValues { get; set; }
    public string targetFile;
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
    //private async void Save(object sender, EventArgs e)
    //{
    //    // Capture the screenshot
    //    IScreenshotResult screenshotResult = await DrawableView.CaptureAsync();

    //    if (screenshotResult != null)
    //    {
    //        // Create an output filename
    //        string targetFile = Path.Combine(FileSystem.AppDataDirectory, "test1.png");

    //        try
    //        {
    //            // Copy the file to the AppDataDirectory
    //            using (FileStream outputStream = File.Create(targetFile))
    //            {
    //                await screenshotResult.CopyToAsync(outputStream);
    //            }

    //            // Display a success message
    //            await DisplayAlert("Success", "Screenshot saved successfully.", "OK");
    //        }
    //        catch (Exception ex)
    //        {
    //            // Display an error message if saving fails
    //            await DisplayAlert("Error", $"Failed to save screenshot: {ex.Message}", "OK");
    //        }
    //    }
    //    else
    //    {
    //        // Display a message if no screenshot was captured
    //        await DisplayAlert("Error", "Failed to capture screenshot.", "OK");
    //    }
    //}
    private async void Save(object sender, EventArgs e)
    {
        // Capture the screenshot
        IScreenshotResult screenshotResult = await DrawableView.CaptureAsync();

        if (screenshotResult != null)
        {
            // Create an output filename
            string targetFile = Path.Combine(FileSystem.AppDataDirectory, "test4.png");

            try
            {
                using (FileStream outputStream = File.Create(targetFile))
                {
                    await screenshotResult.CopyToAsync(outputStream);
                }
              
                    string desktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string desktopFilePath = Path.Combine(desktopDirectory, "test.png");

                // Copy the file from the AppDataDirectory to the desktop
                File.Copy(targetFile, desktopFilePath, true);

                // Display a success message
                await DisplayAlert("Success", "Screenshot saved to desktop successfully.", "OK");
            }
            catch (Exception ex)
            {
                // Display an error message if saving fails
                await DisplayAlert("Error", $"Failed to save screenshot to desktop: {ex.Message}", "OK");
            }
        }
        else
        {
            // Display a message if no screenshot was captured
            await DisplayAlert("Error", "Failed to capture screenshot.", "OK");
        }
    }


    private async void BackToMainPageButton_Clicked(object sender, EventArgs e)
    {
        
        await Navigation.PopModalAsync(); // Navigate back to the MainPage

    }

}



