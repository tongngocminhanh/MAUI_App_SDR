using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Maui.ApplicationModel;


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
        // End properties change
        
        // Binding commands initiate
        public ICommand SelectAndUploadFileCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand UploadParametersCommand { get; }
        public ICommand GenerateOutputCommand { get; }
        public ICommand StartListeningCommand { get; }
        public ICommand StopListeningCommand { get; }

        // Default constructor required for XAML instantiation
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
                        !string.IsNullOrEmpty(ConnectionString) &&
                        !string.IsNullOrEmpty(TableStorageName) &&
                        _entryCellValues != null && _entryCellValues.Length >= 7 &&
                        !_entryCellValues.Take(7).Any(string.IsNullOrEmpty);
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

        // Connecting method to access the storage account
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

        // Define the booolean value to make commands valid
        private bool CanExecuteCommands()
        {
            return IsConnected;
        }

        // Choose a file or multiple files and upload to the defined blob
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

        //Upload the parameters in MainPage to Table Storage
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

        // Proccess the output: access to Storage, take content from Blob, parameters from Table, draw in Page1, upload outputs
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
        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        // From Blob stored SDR files, download files to desktop
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

        // Helper method to parse the file content into a 2D integer array
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

        // Access to Output Blob Storage, download output to desktop
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

        // Start the Message generation
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

        private async Task OnStopListening()
        {
            _cts?.Cancel();
            UpdateMessage("Status: Stopped");
        }

        // Status for Message proccess
        public void UpdateMessage(string newMessage)
        {
            ListenMessage = newMessage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
