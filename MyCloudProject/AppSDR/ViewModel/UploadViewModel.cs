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
        public string _assignedTextFilePath;
        private List<string> _selectedFiles = new List<string>();
        private string[] _entryCellValues;
        private string[] _azureEntryCellValues;
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
        public List<string> SelectedFiles
        {
            get { return _selectedFiles; }
            set
            {
                _selectedFiles = value;
                OnPropertyChanged(nameof(SelectedFiles));
            }
        }
        public string[] EntryCellValues
        {
            get => _entryCellValues;
            set
            {
                _entryCellValues = value;
                OnPropertyChanged();
            }
        }
        public string[] AzureEntryCellValues
        {
            get => _azureEntryCellValues;
            set
            {
                _azureEntryCellValues = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged();
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
                _uploadBlobStorageName = value;
                OnPropertyChanged();
            }
        }

        public string DownloadBlobStorageName
        {
            get => _downloadBlobStorageName;
            set
            {
                _downloadBlobStorageName = value;
                OnPropertyChanged();
            }
        }
        public string TableStorageName
        {
            get => _tableStorageName;
            set
            {
                _tableStorageName = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        public string ListenMessage
        {
            get => _listenMessage;
            set
            {
                if (_listenMessage != value)
                {
                    _listenMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectAndUploadFileCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand UploadParametersCommand { get; }
        public ICommand GenerateOutputCommand { get; }
        public ICommand StartListeningCommand { get; }
        public ICommand StopListeningCommand { get; }

        // Default constructor required for XAML instantiation
        public UploadViewModel(string AssignedTextFilePath, INavigation Navigation, string[] EntryCellValues)
        {
            _assignedTextFilePath = AssignedTextFilePath;
            _navigation = Navigation;
            _entryCellValues = EntryCellValues;

            _selectedFiles = new List<string>(); // Initialize the list here
            ConnectCommand = new Command(async () => await OnConnectAsync());
            UploadParametersCommand = new Command(async () => await UploadParameters(ConnectionString, TableStorageName, _entryCellValues));
            SelectAndUploadFileCommand = new Command(async ()
                => await SelectAndUploadFileAsync(_selectedFiles, AssignedTextFilePath), CanExecuteCommands);
            GenerateOutputCommand = new Command(async ()
                => await GenerateOutput(ConnectionString, UploadBlobStorageName, DownloadBlobStorageName, TableStorageName, _navigation));
            DownloadFilesCommand = new Command(async () => await DownloadFilesAsync(), CanExecuteCommands);
            StartListeningCommand = new Command(async () => await OnStartListening(_navigation));
            StopListeningCommand = new Command(async () => await OnStopListening());
        }

        // Rest of the methods...
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

            ((Command)SelectAndUploadFileCommand).ChangeCanExecute();
            ((Command)DownloadFilesCommand).ChangeCanExecute();
            ((Command)GenerateOutputCommand).ChangeCanExecute();
            ((Command)UploadParametersCommand).ChangeCanExecute();
        }

        private bool CanExecuteCommands()
        {
            return IsConnected;
        }
        //private async Task<string[]> GetParameters()
        //{
        //}
        public async Task UploadParameters(string ConnectionString, string TableStorageName, string[] EntryCellValues)
        {

            if ((EntryCellValues == null) && (EntryCellValues.Length <7))
            {
                // Display a warning message to the user
                StatusMessage = "Warning: Entry cell values are empty or null. Cannot upload parameters.";
            }

            else
            {
                await UploadWhenExist(ConnectionString, TableStorageName, EntryCellValues);
            }
        }

        private async Task UploadWhenExist(string ConnectionString, string TableStorageName, string[] EntryCellValues)
        {
            try
            {
                // Create a table client
                TableClient tableClient = new TableClient(ConnectionString, TableStorageName);

                // Create the table if it doesn't exist
                await tableClient.CreateIfNotExistsAsync();

                // Create an instance of the ConfigurationEntity
                var entity = new TableConfigurationEntity
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
        public async Task<string[]> DownloadEntity(string ConnectionString, string TableStorageName)
        {
            try
            {
                // Create a table client
                TableClient tableClient = new TableClient(ConnectionString, TableStorageName);

                // Query all entities
                Pageable<TableConfigurationEntity> queryResults = tableClient.Query<TableConfigurationEntity>();

                // Convert to a list and order by Timestamp in descending order
                List<TableConfigurationEntity> entities = queryResults.ToList();
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

        public async Task GenerateOutput(string ConnectionString, string UploadBlobStorageName, string DownloadBlobStorageName, string TableStorageName, INavigation Navigation)
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
                        
                        await ProcessDownloadedFileAsync(localFilePath, ConnectionString, DownloadBlobStorageName, TableStorageName, Navigation);


                    }
                    catch (Exception ex)
                    {
                        //UpdateStatusLabel($"Error downloading or processing blob '{blobName}': {ex.Message}");
                        Console.WriteLine($"Error downloading or processing blob '{blobName}': {ex.Message}");
                    }
                }

                //UpdateStatusLabel("All files have been processed.");
                Console.WriteLine($"All files have been processed.");

            }
            catch (Exception ex)
            {
                //UpdateStatusLabel($"Error processing experiment request: {ex.Message}");
                Console.WriteLine($"Error processing experiment request: {ex.Message}");
            }
        }

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

        private async Task ProcessDownloadedFileAsync(string filePath, string connectionString, string downloadBlobStorageName, string tableStorageName, INavigation navigation)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(filePath);
                //UpdateStatusLabel($"Processing file: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Processing file: {Path.GetFileName(filePath)}");

                int[][] activeCellsArray = ParseFileContent(fileContent);
                string[] entryCellValues = await DownloadEntity(ConnectionString, TableStorageName);

                // Ensure the navigation to Page1 is awaited and on the main thread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var page1 = new Page1(activeCellsArray, entryCellValues, connectionString, downloadBlobStorageName, navigation);
                    await navigation.PushModalAsync(page1);

                    // Wait for the screenshot to be captured and uploaded
                    await page1.SaveScreenshotToBlobStorage();


                });
            }
            catch (Exception ex)
            {
                //UpdateStatusLabel($"Error processing file {filePath}: {ex.Message}");
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

        private async Task SelectAndUploadFileAsync(List<string> selectedFiles, string _assignedTextFilePath)
        {
            try
            {
                if (_assignedTextFilePath == null)
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
                            selectedFiles.Add(file.FullPath);
                        }
                        StatusMessage = $"{selectedFiles.Count} files selected.";
                        var containerClient = new BlobContainerClient(ConnectionString, UploadBlobStorageName);
                        await containerClient.CreateIfNotExistsAsync();

                        foreach (var file in selectedFiles)
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

                    var blobClient = containerClient.GetBlobClient(Path.GetFileName(_assignedTextFilePath));
                    await blobClient.UploadAsync(_assignedTextFilePath, overwrite: true);

                    StatusMessage = "File uploaded successfully.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            ((Command)GenerateOutputCommand).ChangeCanExecute();
        }

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

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        public void UpdateMessage(string newMessage)
        {
            ListenMessage = newMessage;
        }

        private async Task OnStartListening(INavigation _navigation)
        {
            if (IsConnected == false && string.IsNullOrEmpty(DownloadBlobStorageName))
            {
                StatusMessage = "Please provide a valid connection string, storage account and blob storage name.";
            }
            else
            {
                UpdateMessage("Status: Start Listening");
                string QueueName = "trigger";
                _listener = new QueueMessageListener(ConnectionString, QueueName, DownloadBlobStorageName, _navigation);
                _cts = new CancellationTokenSource();
                Task.Run(async () => await _listener.ListenToMessagesAsync(_cts.Token));
            }
        }
        private async Task OnStopListening()
        {
            _cts?.Cancel();
            UpdateMessage("Status: Stopped");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
