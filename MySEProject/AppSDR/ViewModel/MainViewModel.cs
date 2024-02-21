using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AppSDR.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private INavigation _navigation;
        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public string SelectedFileName => string.IsNullOrEmpty(SelectedFilePath) ? "Choose a text file" : Path.GetFileName(SelectedFilePath);

        public ICommand ChooseFileCommand { get; }
        public ICommand SubmitCommand { get; }

        public MainViewModel(INavigation navigation)
        {
            ChooseFileCommand = new Command(ChooseFile);
            SubmitCommand = new Command(Submit);
            _navigation = navigation;
        }
        private async void ChooseFile()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Pick a text file"
                });

                if (result != null)
                {
                    SelectedFilePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"File picking error: {ex.Message}");
            }
        }

        private async void Submit()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedFilePath))
                {
                    // Read the text content of the selected file
                    string fileContent = await File.ReadAllTextAsync(SelectedFilePath);

                    // Split the content into lines
                    string[] lines = fileContent.Split(Environment.NewLine);

                    // Initialize a 2D integer array to store the parsed values
                    int[][] activeCellsColumn = new int[lines.Length][];

                    // Iterate over each line and parse the integers
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // Split each line into individual integers
                        string[] numbers = lines[i].Split(',');

                        // Initialize an array to store the parsed integers for this line
                        int[] parsedNumbers = new int[numbers.Length];

                        // Parse each number and store it in the array
                        for (int j = 0; j < numbers.Length; j++)
                        {
                            if (int.TryParse(numbers[j], out int parsedNumber))
                            {
                                parsedNumbers[j] = parsedNumber;
                            }
                            else
                            {
                                // Handle parsing error if needed
                                // For example: throw new ArgumentException("Invalid number format");
                            }
                        }

                        // Store the parsed numbers for this line in the 2D array
                        activeCellsColumn[i] = parsedNumbers;

                      
                    }
                  
                    await Application.Current.MainPage.DisplayAlert("Success", "Data saved successfully", "OK");
                    await _navigation.PushAsync(new Page1(activeCellsColumn));
                    
                }
                else
                {
                    // Display error message if no file is selected
                    await Application.Current.MainPage.DisplayAlert("Error", "Please choose a file", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
