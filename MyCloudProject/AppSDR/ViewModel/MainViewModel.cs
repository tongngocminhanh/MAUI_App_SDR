using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Azure.Storage.Queues;

namespace AppSDR.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Start property change of parameters without Cloud
        private string _graphName;
        public string GraphName
        {
            get => _graphName;
            set
            {
                if (_graphName != value)
                {
                    _graphName = value;
                    OnPropertyChanged(nameof(GraphName)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _savedName;
        public string SavedName
        {
            get => _savedName;
            set
            {
                if (_savedName != value)
                {
                    _savedName = value;
                    OnPropertyChanged(nameof(SavedName)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _maxCycles;
        public string MaxCycles
        {
            get => _maxCycles;
            set
            {
                if (_maxCycles != value)
                {
                    _maxCycles = value;
                    OnPropertyChanged(nameof(MaxCycles)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _highlightTouch;
        public string HighlightTouch
        {
            get => _highlightTouch;
            set
            {
                if (_highlightTouch != value)
                {
                    _highlightTouch = value;
                    OnPropertyChanged(nameof(HighlightTouch)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _yaxisTitle;
        public string YaxisTitle
        {
            get => _yaxisTitle;
            set
            {
                if (_yaxisTitle != value)
                {
                    _yaxisTitle = value;
                    OnPropertyChanged(nameof(YaxisTitle)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _xaxisTitle;
        public string XaxisTitle
        {
            get => _xaxisTitle;
            set
            {
                if (_xaxisTitle != value)
                {
                    _xaxisTitle = value;
                    OnPropertyChanged(nameof(XaxisTitle)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _minRange;
        public string MinRange
        {
            get => _minRange;
            set
            {
                if (_minRange != value)
                {
                    _minRange = value;
                    OnPropertyChanged(nameof(MinRange)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();
                }
            }
        }

        private string _maxRange;
        public string MaxRange
        {
            get => _maxRange;
            set
            {
                if (_maxRange != value)
                {
                    _maxRange = value;
                    OnPropertyChanged(nameof(MaxRange)); // Raise PropertyChanged event
                    (SubmitCommand as Command).ChangeCanExecute();
                    (AddTextCommand as Command).ChangeCanExecute();


                }
            }
        }
        public string[] EntryCellValues
        {
            get { return _entryCellValues; }
            set { SetProperty(ref _entryCellValues, value); }
        }
        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }
        // End property change of parameters without Cloud

        // Without-cloud properties definition
        private string _selectedFilePath;
        public string SelectedFileName => string.IsNullOrEmpty(SelectedFilePath) ? "Choose a text file" : Path.GetFileName(SelectedFilePath);
        public ICommand ChooseFileCommand { get; }
        public ICommand SubmitCommand { private set; get; }
        public ICommand AddTextCommand { private set; get; }

        // Navigation elements
        private INavigation _navigation;
        public INavigation Navigation
        {
            get { return _navigation; }
            set
            {
                _navigation = value;
                OnPropertyChanged(nameof(Navigation));
            }
        }
        private string[] _entryCellValues;

        // Null pre-defined values
        private string _assignedTextFilePath = null;
        private string _connectionString = null;
        private string _downloadBlobStorage = null;
        private string[] _messageConfig = null;
        private string[] _cloudConfig = null;

        // Start property change of Message parameters with Cloud
        private string _queueStorageName;
        public string QueueStorageName
        {
            get => _queueStorageName;
            set
            {
                _queueStorageName = value;
                OnPropertyChanged();;
                (UploadMessageCommand as Command).ChangeCanExecute();
            }
        }
        private string _messageConnectionString;
        public string MessageConnectionString
        {
            get => _messageConnectionString;
            set
            {
                _messageConnectionString = value;
                OnPropertyChanged();
                (UploadMessageCommand as Command).ChangeCanExecute();
            }
        }
        private string _messageContent;
        public string MessageContent
        {
            get => _messageContent;
            set
            {
                _messageContent = value;
                OnPropertyChanged();
                (UploadMessageCommand as Command).ChangeCanExecute();
            }
        }
        // End property change of Message parameters with Cloud

        // With-cloud properties definition
        public ICommand NavigateToUploadPageCommand { get; }
        public ICommand UploadMessageCommand { get; }
        public MainViewModel(INavigation Navigation)
        {
            _navigation = Navigation;

            // Without-cloud commands
            ChooseFileCommand = new Command(ChooseFile);
            AddTextCommand = new Command(
                execute: () =>
                {
                    AddText();
                },

                canExecute: () =>
                {
                    // Check if the first 7 parameters are not null
                    bool firstSevenNotNull =
                        !string.IsNullOrEmpty(GraphName) &&
                        !string.IsNullOrEmpty(MaxCycles) &&
                        !string.IsNullOrEmpty(HighlightTouch) &&
                        !string.IsNullOrEmpty(XaxisTitle) &&
                        !string.IsNullOrEmpty(YaxisTitle) &&
                        !string.IsNullOrEmpty(MinRange) &&
                        !string.IsNullOrEmpty(MaxRange);

                    // Return true if the first 7 parameters are not null and SavedName is either null or not null
                    return firstSevenNotNull && (string.IsNullOrEmpty(SavedName) ||
                                                !string.IsNullOrEmpty(SavedName));
                });

            SubmitCommand = new Command(
                execute: () =>
                {
                    Submit();
                },
                canExecute: () =>
                {
                    // Check if the first 7 parameters are not null
                    bool firstSevenNotNull =
                        !string.IsNullOrEmpty(GraphName) &&
                        !string.IsNullOrEmpty(MaxCycles) &&
                        !string.IsNullOrEmpty(HighlightTouch) &&
                        !string.IsNullOrEmpty(XaxisTitle) &&
                        !string.IsNullOrEmpty(YaxisTitle) &&
                        !string.IsNullOrEmpty(MinRange) &&
                        !string.IsNullOrEmpty(MaxRange);

                    // Return true if the first 7 parameters are not null and SavedName is either null or not null
                    return firstSevenNotNull && (string.IsNullOrEmpty(SavedName) || 
                                                !string.IsNullOrEmpty(SavedName));
                });

            // Cloud Upload functions
            NavigateToUploadPageCommand = new Command(NavigateToUploadPage);
            UploadMessageCommand = new Command(
                execute: () =>
                {
                    UploadMessage();
                },

                canExecute: () =>
                {
                    // Check if the first 7 parameters are not null
                    bool definedParaNotNull =
                        !string.IsNullOrEmpty(MessageConnectionString) &&
                        !string.IsNullOrEmpty(QueueStorageName) &&
                        !string.IsNullOrEmpty(MessageContent);

                    return definedParaNotNull;
                });

        }

        // Pick a file from a local device
        private async void ChooseFile()
        {
            try
            {
                // Compatible with Text files and Comma-separated values (csv) files
                // Define file extension for all supported platforms
                var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { ".txt", ".csv" } },
                    { DevicePlatform.Android, new[] {".txt", ".csv" } },
                    { DevicePlatform.WinUI, new[] { ".txt", ".csv" } },
                    { DevicePlatform.Tizen, new[] { ".txt", ".csv" } },
                    { DevicePlatform.macOS, new[] {".txt", ".csv" } }
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
                await Application.Current.MainPage.DisplayAlert("Error", $"File picking error: {ex.Message}", "OK");
            }
        }

        private async void Submit()
        {
            // Done taking inputs and navigate to the visualisation page (Page1)
            try
            {
                if (!string.IsNullOrEmpty(SelectedFilePath))
                {
                    // Read the text content of the selected file
                    string fileContent = await File.ReadAllTextAsync(SelectedFilePath);

                    // Parse the file content and construct activeCellsArray
                    int[][] activeCellsArray = ParseFileContent(fileContent);
                    string[] EntryCellValues = { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MinRange, MaxRange, SavedName };
                    _cloudConfig = [_connectionString, _downloadBlobStorage];
                    // Navigate to Page1 with the updated EntryCellValues and activeCellsArray
                    await NavigateToPage1(EntryCellValues, activeCellsArray);
                }

                // Exception Alert when no input file is detected 
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
        private async Task NavigateToPage1(string[] _entryCellValues, int[][] _activeCellsArray)
        {
            // Navigate to Page1 with the updated EntryCellValues, activeCellsArray, and reference to MainViewModel
            await Navigation.PushModalAsync(new Page1(_activeCellsArray, _entryCellValues, _cloudConfig, Navigation));
        }
        private int[][] ParseFileContent(string fileContent)
        {
            // Split the content into lines
            string[] lines = fileContent.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

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
                        new ArgumentException("Invalid number format");
                    }
                }

                // Add the parsed numbers to the list if the row has at least one non-zero value
                if (hasNonZeroValue)
                {
                    activeCellsColumn.Add(parsedNumbers.ToArray());
                }
            }
            // Convert the list to a 2D array
            return activeCellsColumn.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Handle any changed property, here is entry parameters
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            // Update and save any changed property
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Cloud-related functions
        // Add manually SDR values
        private async void AddText()
        {
            // Parse EntryCellValues, navigate to Text Editor Page
            string[] EntryCellValues = { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MinRange, MaxRange, SavedName };
            _cloudConfig = [_connectionString, _downloadBlobStorage];
            await Navigation.PushModalAsync(new TextEditorPage(EntryCellValues, _cloudConfig, Navigation));
        }
        
        // Navigate to Upload Page
        private async void NavigateToUploadPage()
        {
            try
            {
                string[] EntryCellValues = { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MinRange, MaxRange, SavedName };
                await Navigation.PushModalAsync(new UploadPage(_assignedTextFilePath, Navigation, EntryCellValues));
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"File picking error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"File picking error: {ex.Message}", "OK");
            }
        }

        // Handle Message used for automatically generating outputs
        private async void UploadMessage()
        {
            try
            {
                await SendMessageToQueue();
                _messageConfig = [MessageConnectionString, QueueStorageName];
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Upload message error: {ex.Message}", "OK");
            }
        }

        private async Task SendMessageToQueue()
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(MessageConnectionString, QueueStorageName);

            try
            {
                // Create the queue if it doesn't already exist
                await queueClient.CreateIfNotExistsAsync();

                if (await queueClient.ExistsAsync())
                {
                    // Send a message to the queue
                    await queueClient.SendMessageAsync(MessageContent);
                    await Application.Current.MainPage.DisplayAlert("Success", "Message sent to queue", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Queue does not exist", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
            }
        }
    }
}