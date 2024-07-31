using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Data.Tables;

namespace TestCloudProject.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _connectionString;
        private string _uploadBlobStorageName;
        private string _downloadBlobStorageName;
        private string _tableStorageName;
        private string _statusMessage;

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged();
            }
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

        public ICommand SelectAndUploadFileCommand { get; }
        public ICommand DownloadFilesCommand { get; }
        public ICommand UploadParametersCommand { get; }

        public MainViewModel()
        {
            SelectAndUploadFileCommand = new Command(async () => await SelectAndUploadFileAsync());
            DownloadFilesCommand = new Command(async () => await DownloadFilesAsync());
            UploadParametersCommand = new Command(async () => await UploadParameters());
        }

        private async Task SelectAndUploadFileAsync()
        {
            try
            {
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
                    var containerClient = new BlobContainerClient(ConnectionString, UploadBlobStorageName);
                    await containerClient.CreateIfNotExistsAsync();

                    foreach (var file in filePickerResult)
                    {
                        var blobClient = containerClient.GetBlobClient(Path.GetFileName(file.FullPath));
                        await blobClient.UploadAsync(file.FullPath, overwrite: true);
                    }

                    StatusMessage = "Files uploaded successfully.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
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

        private async Task DownloadFilesAsync()
        {
            try
            {
                StatusMessage = "Processing experiment request";

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
                StatusMessage = $"Error processing experiment request: {ex.Message}";
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
        private async Task UploadParameters()
        {
            string[] EntryCellValues = { "GraphName", "100", "1", "XaxisTitle", "YaxisTitle", "1", "5000", "SDR" };
            // Create a table client
            TableClient tableClient = new TableClient(ConnectionString, TableStorageName);

            // Create the table if it doesn't exist
            await tableClient.CreateIfNotExistsAsync();

            // Create an instance of the ConfigurationEntity
            var entity = new ConfigurationEntity
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
