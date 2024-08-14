using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Azure.Storage.Queues;

namespace AppSDR.ViewModel
{
    /// <summary>
    /// ViewModel for the Main Page. Manages commands for file selection, text addition, submission, and cloud interactions.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _graphName;

        /// <summary>
        /// Gets or sets the name of the graph, appears on the output.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates GraphName whenever _graphName is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
        public string GraphName
        {
            get => _graphName;
            set
            {
                if (_graphName != value)
                {
                    _graphName = value;
                    OnPropertyChanged(nameof(GraphName)); // Raise PropertyChanged event
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name file of the graph when saved.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates SavedName whenever _savedName is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of graph column (x-axis).
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates MaxCycles whenever _maxCycles is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column, which is highlighted.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates HighlightTouch whenever _highlightTouch is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the y axis name of the graph.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates YaxisTitle whenever _yaxisTitle is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the x axis name of the graph.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates XaxisTitle whenever _xaxisTitle is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum values of SDR values.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates MinRange whenever _minRange is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum values of SDR values.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates MaxRange whenever _maxRange is set.
        /// Validates SubmitCommand and AddTextCommand to execute.
        /// </return>
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
                    (SubmitCommand as Command)?.ChangeCanExecute();
                    (AddTextCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        /// <summary>
        /// Gets or sets the parameters for drawing functions, used in other functions
        /// </summary>
        /// <return>
        /// Updates GraphName whenever _graphName is set.
        /// </return>
        public string[] EntryCellValues
        {
            get { return _entryCellValues; }
            set { SetProperty(ref _entryCellValues, value); }
        }

        /// <summary>
        /// Gets or sets the file directory to use.
        /// </summary>
        /// <return>
        /// Updates SelectedFilePath whenever _selectedFilePath is set.
        /// </return>
        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set
            {
                if (_selectedFilePath != value)
                {
                    _selectedFilePath = value;
                    OnPropertyChanged(nameof(SelectedFilePath)); // Raise PropertyChanged event
                }
            }
        }


        // Without-Cloud commands definition
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

        private string _queueStorageName;

        /// <summary>
        /// Gets or sets the name of the queue storage.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates QueueStorageName whenever _queueStorageName is set.
        /// </return>
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

        /// <summary>
        /// Gets or sets the connection string for the message service.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates MessageConnectionString whenever _messageConnectionString is set.
        /// </return>
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

        /// <summary>
        /// Gets or sets the content of the message to be uploaded.
        /// This property is bound to UI elements and supports change notifications.
        /// </summary>
        /// <return>
        /// Updates MessageContent whenever _messageContent is set.
        /// </return>
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

        // With-Cloud command definition
        public ICommand NavigateToUploadPageCommand { get; }
        public ICommand UploadMessageCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="navigation">The navigation service used to navigate between pages.</param>
        public MainViewModel(INavigation Navigation)
        {
            _navigation = Navigation;

            // Without-Cloud command operating conditions
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
                    // Check if the follow parameters are not null
                    bool definedParaNotNull =
                        !string.IsNullOrEmpty(MessageConnectionString) &&
                        !string.IsNullOrEmpty(QueueStorageName) &&
                        !string.IsNullOrEmpty(MessageContent);
                    // Return True when they are defined
                    return definedParaNotNull;
                });

        }

        /// <summary>
        /// Command to choose a file from the local device.
        /// </summary>
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

        /// <summary>
        /// Command to submit the form data and navigate to the visualization page.
        /// </summary>
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
                    await NavigateToPage1(EntryCellValues, activeCellsArray, _cloudConfig);
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

        /// <summary>
        /// Navigates to the Page1 with the provided parameters and data.
        /// </summary>
        /// <param name="_entryCellValues">Array of graph parameters.</param>
        /// <param name="_activeCellsArray">2D array of parsed cell values from the file.</param>
        /// <param name="_cloudConfig">Array of cloud configuration strings.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task NavigateToPage1(string[] _entryCellValues, int[][] _activeCellsArray, string[] _cloudConfig)
        {
            // Navigate to Page1 with the updated EntryCellValues, activeCellsArray, and reference to MainViewModel
            var page1FromMainPage = new Page1(_activeCellsArray, _entryCellValues, _cloudConfig, Navigation, typeof(MainPage));
            await Navigation.PushModalAsync(page1FromMainPage);
        }

        /// <summary>
        /// Parses the content of the file into a 2D array of integers.
        /// </summary>
        /// <param name="fileContent">Content of the file as a string.</param>
        /// <returns>A 2D array of integers parsed from the file content.</returns>
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

        /// <summary>
        /// Raises the PropertyChanged event for data binding.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
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

        /// <summary>
        /// Command to add text and navigate to the text editor page.
        /// </summary>
        private async void AddText()
        {
            // Parse EntryCellValues, navigate to Text Editor Page
            string[] EntryCellValues = { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MinRange, MaxRange, SavedName };
            _cloudConfig = [_connectionString, _downloadBlobStorage];
            await Navigation.PushModalAsync(new TextEditorPage(EntryCellValues, _cloudConfig, _messageConfig, Navigation));
        }

        /// <summary>
        /// Command to navigate to the upload page.
        /// </summary>
        private async void NavigateToUploadPage()
        {
            try
            {
                // Parse EntryCellValues, can be null, navigate to Text Editor Page
                string[] EntryCellValues = { GraphName, MaxCycles, HighlightTouch, XaxisTitle, YaxisTitle, MinRange, MaxRange, SavedName };
                await Navigation.PushModalAsync(new UploadPage(_assignedTextFilePath, _messageConfig, Navigation, EntryCellValues));
            }
            catch (Exception ex)
            {
                // Handle exception
                await Application.Current.MainPage.DisplayAlert("Error", $"File picking error: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Command to upload a message to the cloud queue.
        /// </summary>
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

        /// <summary>
        /// Sends a message to the cloud queue.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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