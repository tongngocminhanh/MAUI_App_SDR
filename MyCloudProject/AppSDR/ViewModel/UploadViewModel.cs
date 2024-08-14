using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AppSDR.ViewModel
{
    public class UploadViewModel : INotifyPropertyChanged
    {
        // Fields and properties
        private INavigation _navigation;
        private string _assignedTextFilePath;
        private string[] _messageConfig;
        private List<string> _selectedFiles = new List<string>();
        private string[] _entryCellValues;
        private bool _isConnected;
        private string _connectionString;
        private string _storageAccount;
        private string _uploadBlobStorageName;
        private string _downloadBlobStorageName;
        private string _tableStorageName;
        private string _statusMessage;
        private string _listenMessage = "Message mode: Off";
        private QueueMessageListener _listener;
        private CancellationTokenSource _cts;
        
        // Start properties change
        public INavigation Navigation
        {
            get { return _navigation; }
            set
            {
                _navigation = value; OnPropertyChanged(nameof(Navigation));
            }
        }
        public string AssignedTextFilePath
        {
            get { return _assignedTextFilePath; }
            set
            {
                _assignedTextFilePath = value; OnPropertyChanged(nameof(AssignedTextFilePath));
            }
        }
        public string[] MessageConfig
        {
            get { return _messageConfig; }
            set
            {
                _messageConfig = value; OnPropertyChanged(nameof(MessageConfig));
            }
        }
        public List<string> SelectedFiles
        {
            get { return _selectedFiles; }
            set
            {
                _selectedFiles = value; OnPropertyChanged(nameof(SelectedFiles));
            }
        }
        public string[] EntryCellValues
        {
            get => _entryCellValues;
            set
            {
                _entryCellValues = value; OnPropertyChanged();
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value; OnPropertyChanged();
            }
        }

        public string StorageAccount
        {
            get => _storageAccount;
            set { _storageAccount = value; OnPropertyChanged(); }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); }
        }

        public string UploadBlobStorageName
        {
            get => _uploadBlobStorageName;
            set
            {
                _uploadBlobStorageName = value; OnPropertyChanged();
            }
        }

        public string DownloadBlobStorageName
        {
            get => _downloadBlobStorageName;
            set
            {
                _downloadBlobStorageName = value; OnPropertyChanged();
            }
        }
        public string TableStorageName
        {
            get => _tableStorageName;
            set
            {
                _tableStorageName = value; OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value; OnPropertyChanged();
            }
        }
        public string ListenMessage
        {
            get => _listenMessage;
            set
            {
                _listenMessage = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initialize commands defined in UploadPage UI. 
        /// </summary>
        /// <param ICommand="Binding">The binding commands from UploadPage UI</param>
        /// <returns>The commands method of UploadPage UI</returns>
        public ICommand SelectAndUploadFileCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand UploadParametersCommand { get; }
        public ICommand GenerateOutputCommand { get; }
        public ICommand StartListeningCommand { get; }
        public ICommand StopListeningCommand { get; }


        /// <summary>
        /// Initialize the parse variables . 
        /// Initialize the commands binding from the UI - UploadPage.xaml.
        /// </summary>
        /// <param name="AssignedTextFilePath">The input file directory, of saved text file from Text Editor Page, or null.</param>
        /// <param name="MessageConfig">A list contains a Connection String and Queue Storage name, if defined in Main Page, or null</param>
        /// <param name="Navigation">The navigation method to move among pages</param>
        /// <returns>A class behind Upload() deals with logic implementation</returns>
        public UploadViewModel(string AssignedTextFilePath, string[] MessageConfig, INavigation Navigation, string[] EntryCellValues)
        {
            // Assign navigating variables
            _assignedTextFilePath = AssignedTextFilePath;
            _messageConfig = MessageConfig;
            _navigation = Navigation;
            _entryCellValues = EntryCellValues;

            // Define command conditions and execution
            ConnectCommand = new Command(async () => await OnConnectAsync());
            SelectAndUploadFileCommand = new Command(async ()
                => await SelectAndUploadFileAsync(), CanExecuteCommands);

            UploadParametersCommand = new Command(
                execute: async() =>
                {
                    await UploadParameters();
                },
                canExecute: () =>
                {
                    // Check if the required parameters are not null
                    bool definedParaNotNull =
                        CanExecuteCommands() &&
                        !_entryCellValues.Take(8).Any(string.IsNullOrEmpty);
                    return definedParaNotNull;
                });

            GenerateOutputCommand = new Command(
                execute: async () =>
                {
                    await GenerateOutput();
                },
                canExecute: () =>
                {
                    // Check if the defined parameters are not null
                    bool canExecuteCommand = CanExecuteCommands();
                    return canExecuteCommand;
                });

            DownloadFilesCommand = new Command(
                execute: async () =>
                {
                    await DownloadFilesAsync();
                },
                canExecute: () =>
                {
                    // Check if the defined parameters are not null
                    bool canExecuteCommand = CanExecuteCommands();
                    return canExecuteCommand;
                });

            StartListeningCommand = new Command(
                execute: async () =>
                {
                    await OnStartListening();
                },
                canExecute: () =>
                {
                    // Check if the required parameters are not null
                    bool definedParaNotNull =
                        _messageConfig != null && _messageConfig.Length == 2 &&
                        !_messageConfig.Take(2).Any(string.IsNullOrEmpty);
                    return definedParaNotNull;
                }); 

            StopListeningCommand = new Command(async () => await OnStopListening());
        }

        /// <summary>
        /// Connect to Storage Account 1.
        /// Change the required conditions for other commands to true.
        /// </summary>
        /// <param name="ConnectionString">The connection string use in Cloud connection</param>
        /// <param name="StorageAccount">The storage account name, additionally to the connection string</param>
        /// <param name="StatusMessage">A text appears on UI to show the which function the app processes</param>
        /// <returns>A task connects the Storange Account 1 and prepares conditions for other commands</returns>
        private async Task OnConnectAsync()
        {
            StatusMessage = "Connecting...";
            await Task.Delay(1000); // Simulate a delay for connecting

            // Actual connection logic
            if (!string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrEmpty(StorageAccount))
            {
                IsConnected = true;
                StatusMessage = "Connected to storage account.";
            }
            else
            {
                IsConnected = false;
                StatusMessage = "Please provide a valid connection string and storage account.";
            }

            // Only when connecting to Storage, can the commands processible
            ((Command)SelectAndUploadFileCommand).ChangeCanExecute();
            ((Command)UploadParametersCommand).ChangeCanExecute();
            ((Command)GenerateOutputCommand).ChangeCanExecute();
            ((Command)DownloadFilesCommand).ChangeCanExecute();
        }

        /// <summary>
        /// Condition check for others command.
        /// </summary>
        /// <param>No needed parameters</param>
        /// <returns>The boolean value to make commands valid</returns>
        private bool CanExecuteCommands()
        {
            return IsConnected;
        }

        /// <summary>
        /// Select and upload file(s) to Azure Blob Storage. The method checks if a specific text file is assigned for upload.
        /// If no file is assigned, it allows the user to select one or multiple files for upload.
        /// </summary>
        /// <param name="AssignedTextFilePath">The predefined file path, if a specific file is set for upload.</param>
        /// <param name="ConnectionString">The connection string used to connect to the Azure Storage Account.</param>
        /// <param name="UploadBlobStorageName">The name of the Blob storage container where files will be uploaded.</param>
        /// <param name="SelectedFiles">A list containing the paths of files selected by the user for upload.</param>
        /// <param name="StatusMessage">A text that appears on the UI to indicate the current status of the operation, such as file selection or upload success.</param>
        /// <returns>Uploads the selected or assigned file(s) to the defined Blob storage container in Azure.</returns>

        private async Task SelectAndUploadFileAsync()
        {
            try
            {
                if (AssignedTextFilePath == null)
                {
                    // File selection logic
                    var customFileType = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.iOS, new[] { ".txt", ".csv" } },
                            { DevicePlatform.Android, new[] { ".txt", ".csv" } },
                            { DevicePlatform.WinUI, new[] { ".txt", ".csv" } },
                            { DevicePlatform.Tizen, new[] { ".txt", ".csv" } },
                            { DevicePlatform.macOS, new[] { ".txt", ".csv" } }
                        });

                    var filePickerResult = await FilePicker.PickMultipleAsync(new PickOptions
                    {
                        FileTypes = customFileType,
                        PickerTitle = "Select files to upload"
                    });

                    if (filePickerResult != null)
                    {
                        foreach (var file in filePickerResult)
                        {
                            SelectedFiles.Add(file.FullPath);
                        }
                        StatusMessage = $"{SelectedFiles.Count} files selected.";
                        var containerClient = new BlobContainerClient(ConnectionString, UploadBlobStorageName);
                        await containerClient.CreateIfNotExistsAsync();

                        foreach (var file in SelectedFiles)
                        {
                            var blobClient = containerClient.GetBlobClient(Path.GetFileName(file));
                            await blobClient.UploadAsync(file, overwrite: true);
                        }

                        StatusMessage = "Files uploaded successfully.";
                    }
                }
                else
                {
                    StatusMessage = "The assigned text file is waiting for upload.";
                    var containerClient = new BlobContainerClient(ConnectionString, UploadBlobStorageName);
                    await containerClient.CreateIfNotExistsAsync();

                    var blobClient = containerClient.GetBlobClient(Path.GetFileName(AssignedTextFilePath));
                    await blobClient.UploadAsync(AssignedTextFilePath, overwrite: true);

                    StatusMessage = "File uploaded successfully.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Upload configuration parameters to Table Storage. This method creates a table if it doesn't exist 
        /// Inserts a configuration entity with the provided parameters.
        /// </summary>
        /// <param name="ConnectionString">The connection string used to connect to the Azure Storage Account.</param>
        /// <param name="TableStorageName">The name of the Table Storage where the configuration entity will be stored.</param>
        /// <param name="EntryCellValues">An array containing the eight parameters entered: GraphName, MaxCycles, HighlighTouch, XaxisTitle,
        /// YaxisTitle, MinRange, MaxRange, SavedName.</param>
        /// <param name="StatusMessage">A text that appears on the UI to indicate the current status of the operation</param>
        /// <returns>Uploads configuration entities in the specified Azure Table Storage.</returns>
        private async Task UploadParameters()
        {
            try
            {
                // Create a table client
                TableClient tableClient = new TableClient(ConnectionString, TableStorageName);

                // Create the table if it doesn't exist
                await tableClient.CreateIfNotExistsAsync();

                // Create an instance of the ConfigurationEntity
                var entity = new TableEntityConfiguration
                {
                    PartitionKey = "Configuration",
                    RowKey = Guid.NewGuid().ToString(), // Unique identifier for the entity
                    GraphName = EntryCellValues[0],
                    MaxCycles = EntryCellValues[1],
                    HighlightTouch = EntryCellValues[2],
                    XaxisTitle = EntryCellValues[3],
                    YaxisTitle = EntryCellValues[4],
                    MinRange = EntryCellValues[5],
                    MaxRange = EntryCellValues[6],
                    SavedName = EntryCellValues[7]
                };

                // Insert or update the entity
                await tableClient.UpsertEntityAsync(entity);
                StatusMessage = $"Successfully upload entities";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error uploading Entities: {ex.Message}";
            }
        }

        /// <summary>
        /// Generates output by accessing the specified Blob Container, downloading each blob,and processing the downloaded files.
        /// Handles file operations on the user's Desktop.
        /// </summary>
        /// <param name="ConnectionString">The connection string used to connect to the Azure Blob Storage Account.</param>
        /// <param name="UploadBlobStorageName">The name of the Blob Storage container where the blobs are stored for processing.</param>
        /// <param name="StatusMessage">A text that appears on the UI to indicate the current status of the operation.</param>
        /// <returns>The Task calls Downloads blobs to the user's Desktop, processes the files methods.</returns>
        private async Task GenerateOutput()
        {
            try
            {
                // Initialize the BlobServiceClient with the connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(UploadBlobStorageName);

                // Ensure the container exists
                if (!await containerClient.ExistsAsync())
                {
                    StatusMessage = "The specified container does not exist.";
                    return;
                }

                // Iterate through each blob in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    string blobName = blobItem.Name;

                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string localFilePath = Path.Combine(desktopPath, SanitizeFileName(blobName));
                    string directoryPath = Path.GetDirectoryName(localFilePath);

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    try
                    {
                        await DownloadBlobToFileAsync(blobClient, localFilePath);
                        await ProcessDownloadedFileAsync(localFilePath);
                    }
                    catch (Exception ex)
                    {
                        //UpdateStatusLabel($"Error downloading or processing blob '{blobName}': {ex.Message}");
                        Console.WriteLine($"Error downloading or processing blob '{blobName}': {ex.Message}");
                    }
                }
                Console.WriteLine($"All files have been processed.");

            }
            catch (Exception ex)
            {
                //UpdateStatusLabel($"Error processing experiment request: {ex.Message}");
                Console.WriteLine($"Error processing experiment request: {ex.Message}");
            }
        }

        /// <summary>
        /// Sanitizes a file name by replacing any invalid characters with an underscore.
        /// </summary>
        /// <param name="fileName">The original file name that may contain invalid characters.</param>
        /// <returns>A sanitized file name with invalid characters replaced by underscores.</returns>
        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        /// <summary>
        /// Downloads a blob from Azure Blob Storage and saves it to the specified local file path on the user's machine.
        /// </summary>
        /// <param name="blobClient">The BlobClient used to download the blob from the Azure storage container.</param>
        /// <param name="filePath">The full local file path where the blob will be saved.</param>
        /// <returns>Downloads the blob to a file, and saves to the Desktop.</returns>
        private async Task DownloadBlobToFileAsync(BlobClient blobClient, string filePath)
        {
            try
            {
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await download.Content.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading blob to file {filePath}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Processes a downloaded file by reading its content, parsing it, and updating the UI with the processed data.
        /// Also, navigates to Page1 after processing the file.
        /// </summary>
        /// <param name="filePath">The local file path of the downloaded file that needs to be processed.</param>
        /// <returns>Processes the file and navigates to a new page with the parsed data.</returns>
        private async Task ProcessDownloadedFileAsync(string filePath)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(filePath);
                //UpdateStatusLabel($"Processing file: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Processing file: {Path.GetFileName(filePath)}");

                int[][] activeCellsArray = ParseFileContent(fileContent);
                string[] entryCellValues = await DownloadEntity();

                // Ensure the navigation to Page1 is awaited and on the main thread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    string[] cloudConfig = [ConnectionString, DownloadBlobStorageName];
                    var page1FromUploadPage = new Page1(activeCellsArray, entryCellValues, cloudConfig, Navigation, typeof(UploadPage));
                    await Navigation.PushModalAsync(page1FromUploadPage);

                    // Wait for the screenshot to be captured and uploaded
                    await page1FromUploadPage.SaveScreenshotToBlobStorage();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading blob to file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses the content of a file into a 2D integer array.
        /// Empty lines or lines with invalid data are skipped.
        /// </summary>
        /// <param name="fileContent">The content of the file to be parsed.</param>
        /// <returns>A 2D integer array representing the parsed data from the file.</returns>
        private int[][] ParseFileContent(string fileContent)
        {
            string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<int[]> activeCellsColumn = new List<int[]>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] numbers = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                List<int> parsedNumbers = new List<int>();
                bool hasNonZeroValue = false;

                foreach (string numberString in numbers)
                {
                    string trimmedNumberString = numberString.Trim();
                    if (string.IsNullOrEmpty(trimmedNumberString))
                    {
                        continue;
                    }

                    if (int.TryParse(trimmedNumberString, out int parsedNumber))
                    {
                        if (parsedNumbers.Count == 0 && parsedNumber == 0)
                        {
                            hasNonZeroValue = false;
                            break;
                        }

                        parsedNumbers.Add(parsedNumber);

                        if (!hasNonZeroValue && parsedNumber != 0)
                        {
                            hasNonZeroValue = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing line: '{line}'. Value: '{trimmedNumberString}' is not a valid number.");
                        StatusMessage = "The specified container does not exist.";
                    }
                }
                if (hasNonZeroValue)
                {
                    activeCellsColumn.Add(parsedNumbers.ToArray());
                }
            }
            return activeCellsColumn.ToArray();
        }

        // Access to Table Storage, and get Entry Cell Values
        /// <summary>
        /// Downloads the most recent configuration entity from Azure Table Storage and converts its properties into a string array.
        /// </summary>
        /// <param name="ConnectionString">The connection string used to connect to the Azure Storage Account.</param>
        /// <param name="TableStorageName">The name of the Table Storage where the configuration entity will be stored.</param>
        /// <returns>A string array representing the configuration entity's properties.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no entities are found in the table.</exception>
        public async Task<string[]> DownloadEntity()
        {
            try
            {
                // Create a table client
                TableClient tableClient = new TableClient(ConnectionString, TableStorageName);

                // Query all entities
                Pageable<TableEntityConfiguration> queryResults = tableClient.Query<TableEntityConfiguration>();

                // Convert to a list and order by Timestamp in descending order
                List<TableEntityConfiguration> entities = queryResults.ToList();
                var mostRecentEntity = entities.OrderByDescending(ent => ent.Timestamp).FirstOrDefault();

                if (mostRecentEntity != null)
                {
                    // Convert the entity properties back to a string array
                    EntryCellValues = new string[]
                    {
                        mostRecentEntity.GraphName,
                        mostRecentEntity.MaxCycles,
                        mostRecentEntity.HighlightTouch,
                        mostRecentEntity.XaxisTitle,
                        mostRecentEntity.YaxisTitle,
                        mostRecentEntity.MinRange,
                        mostRecentEntity.MaxRange,
                        mostRecentEntity.SavedName
                    };

                    return EntryCellValues;
                }
                else
                {
                    // Handle case when no entities are found
                    throw new InvalidOperationException("No entities found in the table.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                StatusMessage = $"Error downloading entity: {ex.Message}";
                throw; // Optionally rethrow the exception or return a default value
            }
        }

        /// <summary>
        /// Downloads all files from a specified Blob Storage container to the user's Desktop. Each file is sanitized and saved locally.
        /// Handles potential errors during the download process and updates the status message accordingly.
        /// </summary>
        /// <param name="ConnectionString">The connection string used to connect to the Azure Blob Storage Account.</param>
        /// <param name="DownloadBlobStorageName">The name of the Blob Storage container where output images are stored.</param>
        /// <param name="StatusMessage">A text that appears on the UI to indicate the current status of the operation.</param>
        /// <returns>Downloads files from the Blob Storage container and saves them to the Desktop, while providing status updates.</returns>
        private async Task DownloadFilesAsync()
        {
            try
            {
                StatusMessage = "Processing download request";

                BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(DownloadBlobStorageName);

                if (!await containerClient.ExistsAsync())
                {
                    StatusMessage = "Container does not exist.";
                    return;
                }

                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    string blobName = blobItem.Name;
                    StatusMessage = $"Found blob: {blobName}";

                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    string localFilePath = Path.Combine(desktopPath, SanitizeFileName(blobName));

                    try
                    {
                        await DownloadBlobToFileAsync(blobClient, localFilePath);
                        StatusMessage = $"Downloaded file to {localFilePath}";
                    }
                    catch (Exception ex)
                    {
                        StatusMessage = $"Error downloading or processing blob '{blobName}': {ex.Message}";
                    }
                }
                StatusMessage = "All files have been processed.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error processing download request: {ex.Message}";
            }
        }

        /// <summary>
        /// Starts listening to Azure Queue Messages using the configured MessageConfig object.
        /// Displays a status update and begins listening for messages asynchronously. Handles any exceptions that occur during the process.
        /// </summary>
        /// <param name="MessageConfig">A list contains a Connection String and Queue Storage name, if defined in Main Page</param>
        /// <returns>Begins listening to messages from the queue and updates the UI with status information.</returns>
        private async Task OnStartListening()
        {
            if (MessageConfig == null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusMessage = "Please provide a valid connection string, queue name, and message content in Main Page";
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateMessage("Status: Start Listening");
                });

                _listener = new QueueMessageListener(MessageConfig, Navigation);
                _cts = new CancellationTokenSource();

                await Task.Run(async () =>
                {
                    try
                    {
                        await _listener.ListenToMessagesAsync(_cts.Token);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions here
                        Console.WriteLine($"Error: {ex.Message}");
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage.DisplayAlert("Error", $"Fail to receive messages: {ex.Message}", "OK");
                        });
                    }
                });
            }
        }

        /// <summary>
        /// Stops listening to Azure Queue Messages by canceling the CancellationTokenSource.
        /// Updates the status message to indicate that listening has stopped.
        /// </summary>
        /// <param name="_cts">CancellationTokenSource object, provides cancellation token through its Token property.</param>
        /// <returns>Stops the message listening operation and updates the UI with the new status.</returns>
        private async Task OnStopListening()
        {
            _cts?.Cancel();
            UpdateMessage("Status: Stopped");
        }

        /// <summary>
        /// Changes the ListenMessage on Upload Page.
        /// </summary>
        /// <param name="newMessage">The content is defined to be shown on Upload Page UI.</param>
        /// <returns>Update the message on the screen so users can keep track of the process.</returns>
        public void UpdateMessage(string newMessage)
        {
            ListenMessage = newMessage;
        }

        /// <summary>
        /// Event that is raised when a property value changes, typically used for data binding.
        /// Implemented from the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes the PropertyChanged event handler to notify the UI or other components that a property value has changed.
        /// This method helps in maintaining dynamic data updates within the UI when properties in the view model change.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that has changed. 
        /// The [CallerMemberName] attribute automatically captures the name of the calling property if no argument is provided.
        /// </param>
        /// <return>Updates the called parameter</return>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
