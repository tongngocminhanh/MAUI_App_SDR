using Microsoft.Maui.Controls;
using System;
using System.IO;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void AddFileButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Open file picker
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Pick a text file"
                });

                if (result != null)
                {
                    // Check if the selected file is a text file
                    if (result.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        // Update button text to indicate file received
                        AddFileButton.Text = "Received";
                    }
                    else
                    {
                        // Update button text to indicate error
                        AddFileButton.Text = "Error";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error picking file: {ex.Message}");
            }
        }
    }
}
