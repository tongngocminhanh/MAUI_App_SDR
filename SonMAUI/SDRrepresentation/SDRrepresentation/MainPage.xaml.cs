using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Data;

namespace SDRrepresentation;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourCsvFile.csv");
        string[][] csvData = ReadCsvFile(filePath);
    }
    private string[][] ReadCsvFile(string filePath)
    {
        List<string[]> data = new List<string[]>();

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    data.Add(rows);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately, e.g., display an error message
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }

        return data.ToArray();
    }
}
    


