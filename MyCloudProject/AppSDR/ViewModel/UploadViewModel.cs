using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AppSDR.ViewModel
{
    public class UploadViewModel : INotifyPropertyChanged
    {
        // Fields and properties
        private INavigation _navigation;
        public string _assignedTextFilePath { get; set; }
        private List<string> _selectedFiles = new List<string>();
        private bool _isConnected;
        private string _connectionString;
        private string _storageAccount;
        private string _uploadBlobStorageName;
        private string _downloadBlobStorageName;
        private string _statusMessage;
        private string _listenMessage = "Message mode: Off";
        private QueueMessageListener _listener;
        private CancellationTokenSource _cts;

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
        public ICommand GenerateOutputCommand { get; }
        public ICommand StartListeningCommand { get; }
        public ICommand StopListeningCommand { get; }

        // Default constructor required for XAML instantiation
        public UploadViewModel(string AssignedTextFilePath, INavigation Navigation)
        {
            _assignedTextFilePath = AssignedTextFilePath;
            _navigation = Navigation;
            _selectedFiles = new List<string>(); // Initialize the list here
            ConnectCommand = new Command(async () => await OnConnectAsync());
            SelectAndUploadFileCommand = new Command(async () 
                => await SelectAndUploadFileAsync(_selectedFiles, _assignedTextFilePath), CanExecuteCommands);
            GenerateOutputCommand = new Command(async () 
                => await GenerateOutput(ConnectionString, UploadBlobStorageName, DownloadBlobStorageName, _navigation));
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
        }

        private bool CanExecuteCommands()
        {
            return IsConnected;
        }

        public async Task GenerateOutput(string ConnectionString, string UploadBlobStorageName, string DownloadBlobStorageName, INavigation navigation)
        {
            try
            {
                // Initialize the BlobServiceClient with the connection string
                var blobServiceClient = new BlobServiceClient(ConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(UploadBlobStorageName);

                // Ensure the container exists
                if (!await containerClient.ExistsAsync())
                {
                    StatusMessage = "The specified container does not exist.";
                    return;
                }

                // Iterate through each blob in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    StatusMessage = $"Processing blob: {blobItem.Name}";

                    // Download the blob's content as a string
                    var blobDownloadInfo = await blobClient.DownloadAsync();
                    using (var streamReader = new StreamReader(blobDownloadInfo.Value.Content))
                    {
                        string fileContent = await streamReader.ReadToEndAsync();

                        // Parse the file content into a 2D integer array
                        int[][] activeCellsArray = ParseFileContent(fileContent);
                        string[] entryCellValues = { "GraphName", "100", "1", "XaxisTitle", "YaxisTitle", "1", "5000", "SDR" };

                        // Navigate to the new page and wait for the screenshot capture
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            var page1 = new Page1(activeCellsArray, entryCellValues, ConnectionString, DownloadBlobStorageName, _navigation);
                            await navigation.PushModalAsync(page1);

                            // Wait for the screenshot to be captured and uploaded
                            await page1.SaveScreenshotToBlobStorage();
                            StatusMessage = "Successfully generate and upload";
                        });

                        // Close the modal page after the screenshot is saved
                        await navigation.PopModalAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error processing blobs: {ex.Message}";
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
                _listener = new QueueMessageListener(ConnectionString, StorageAccount, DownloadBlobStorageName, _navigation);
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
