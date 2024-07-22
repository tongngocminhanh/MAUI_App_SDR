using AppSDR.ViewModel;
using System.ComponentModel;

namespace AppSDR;
public partial class Page1 : ContentPage
{
    public string[] EntryCellValues { get; set; }
    //double WidthRequest { get; set; }
    //double HeightRequest { get; set; }
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
        HeightRequest = graphicsView.HeightRequest;

        // Define horizontal screen size based on total SDR columns
        int max_xvalue = activeCellsColumn.Length;

        if (max_xvalue < 31)
        {
            Rectwidth = 20;
            Rectspacing = 10;
            
            graphicsView.WidthRequest = 350 + max_xvalue*(Rectwidth + Rectspacing);
            WidthRequest = graphicsView.WidthRequest;
        }
        else
        {
            Rectwidth = 5;
            Rectspacing = 2;
            if (max_xvalue < 5100)
            {
                graphicsView.WidthRequest = 200 + max_xvalue * (Rectwidth + Rectspacing);
                WidthRequest = graphicsView.WidthRequest;
            }
            else
            {
                // Warning message of the significant rows of data
                DisplayAlert("File Index Alert", "The file index exceeds the width of the view.", "OK");
            }
        }

        // Assign extra variables in graphicsdrawable
        graphicsdrawable.widthRequest = (float)WidthRequest;
        graphicsdrawable.heightRequest = (float)HeightRequest;
        graphicsdrawable.rectangleSpacing = Rectspacing;
        graphicsdrawable.rectangleWidth = Rectwidth;
        graphicsView.Invalidate();
    }
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
                string desktopFilePath = Path.Combine(desktopDirectory, $"{EntryCellValues[7]}.png");

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
        await Navigation.PushModalAsync(new MainPage()); // Navigate back to the MainPage
    }
}