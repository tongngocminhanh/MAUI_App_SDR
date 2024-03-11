using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppSDR.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private INavigation _navigation;
        private string _selectedFilePath;
        private string[] entryCellValues;

        // Set property change for entry parameters in table
        private string _graphName;
        public string GraphName
        {
            get { return _graphName; }
            set { SetProperty(ref _graphName, value); }
        }

        private string _maxCycles;
        public string MaxCycles
        {
            get { return _maxCycles; }
            set { SetProperty(ref _maxCycles, value); }
        }

        private string _highlightTouch;
        public string HighlightTouch
        {
            get { return _highlightTouch; }
            set { SetProperty(ref _highlightTouch, value); }
        }

        private string _yaxisTitle;
        public string YaxisTitle
        {
            get { return _yaxisTitle; }
            set { SetProperty(ref _yaxisTitle, value); }
        }

        private string _xaxisTitle;
        public string XaxisTitle
        {
            get { return _xaxisTitle; }
            set { SetProperty(ref _xaxisTitle, value); }
        }

        private string _minRange;
        public string MinRange
        {
            get { return _minRange; }
            set { SetProperty(ref _minRange, value); }
        }

        private string _maxRange;
        public string MaxRange
        {
            get { return _maxRange; }
            set { SetProperty(ref _maxRange, value); }
        }

        private string _figureName;
        public string FigureName
        {
            get { return _figureName; }
            set { SetProperty(ref _figureName, value); }
        }
        // End property change
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
        public ICommand AddTextCommand { get; }

        public MainViewModel(INavigation navigation)
        {
            ChooseFileCommand = new Command(ChooseFile);
            SubmitCommand = new Command(Submit);
            _navigation = navigation;
            entryCellValues = new string[] { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MaxRange, MinRange, FigureName };
            AddTextCommand = new Command(() => AddText(entryCellValues));
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
                    FileTypes = customFileType,

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

        // SetProperty method to simplify property setters
        protected bool SetProperty<T>(ref T backingStore, T value,
                                       [CallerMemberName] string propertyName = "",
                                       Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        private async void AddText(string[] entryCellValues)
        {
            await _navigation.PushAsync(new NavigationPage(new TextEditorPage(entryCellValues)));
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

                    // Initialize a list to store the parsed rows
                    List<int[]> activeCellsColumn = new List<int[]>();

                    // Iterate over each line and parse the integers
                    foreach (string line in lines)
                    {
                        // Split each line into individual integers
                        string[] numbers = line.Split(',');

                        // Parse each number and store it in a list
                        List<int> parsedNumbers = new List<int>();

                        // Flag to indicate if the row has at least one non-zero value
                        bool hasNonZeroValue = false;

                        foreach (string numberString in numbers)
                        {
                            if (int.TryParse(numberString, out int parsedNumber))
                            {
                                // Check if it's the first value in the row
                                if (parsedNumbers.Count == 0 && parsedNumber == 0)
                                {
                                    // Skip the row if the first value is 0
                                    hasNonZeroValue = false;
                                    break;
                                }

                                parsedNumbers.Add(parsedNumber);

                                // Set the flag if a non-zero value is found
                                if (!hasNonZeroValue && parsedNumber != 0)
                                {
                                    hasNonZeroValue = true;
                                }
                            }
                            else
                            {
                                // Handle parsing error if needed
                                // For example: throw new ArgumentException("Invalid number format");
                            }
                        }

                        // Add the parsed numbers to the list if the row has at least one non-zero value
                        if (hasNonZeroValue)
                        {
                            activeCellsColumn.Add(parsedNumbers.ToArray());
                        }
                    }

                    // Convert the list to a 2D array
                    int[][] activeCellsArray = activeCellsColumn.ToArray();

                    await Application.Current.MainPage.DisplayAlert("Success", "Data saved successfully", "OK");
                    await _navigation.PushAsync(new NavigationPage(new Page1(activeCellsArray, entryCellValues)));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please select a file", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}