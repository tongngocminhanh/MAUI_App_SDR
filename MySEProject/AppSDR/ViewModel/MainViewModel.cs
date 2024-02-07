using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;


namespace AppSDR.ViewModel
{
    public class MainViewModel : ObservableObject
    {
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

        public MainViewModel()
        {
            ChooseFileCommand = new Command(ChooseFile);
            SubmitCommand = new Command(Submit);
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

        private void Submit()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedFilePath))
                {
                    // Save data using the SelectedFilePath
                    // Implement your saving logic here
                    // For example:
                    // File.WriteAllText(SelectedFilePath, "Your data");

                    // Display success message
                    Application.Current.MainPage.DisplayAlert("Success", "Data saved successfully", "OK");
                }
                else
                {
                    // Display error message if no file is selected
                    Application.Current.MainPage.DisplayAlert("Error", "Please choose a file", "OK");
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
