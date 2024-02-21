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
                var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { ".txt", ".csv" } }, // UTType values
                    { DevicePlatform.Android, new[] {".txt", ".csv" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".txt", ".csv" } }, // file extension
                    { DevicePlatform.Tizen, new[] { ".txt", ".csv" } },
                    { DevicePlatform.macOS, new[] {".txt", ".csv" } }, // UTType values
                });


                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes= customFileType,
                     
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
                    // Save data using the SelectedFilePath
                    // Implement your saving logic here
                    // For example:
                    // File.WriteAllText(SelectedFilePath, "Your data");

                    // Display success message
                    await Application.Current.MainPage.DisplayAlert("Success", "Data saved successfully", "OK");
                    await _navigation.PushAsync(new Page1());
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
