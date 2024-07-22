using Azure.Storage.Blobs;


namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        private List<string> _selectedFiles;

        public string ConnectionString { get; set; }
        public string QueueName { get; set; }

        public UploadPage(string connectionString, string queueName)
        {
            InitializeComponent(); // This will ensure the XAML is loaded properly

            _selectedFiles = new List<string>();
            ConnectionString = connectionString;
            QueueName = queueName;
        }

        private async void OnSelectFilesClicked(object sender, EventArgs e)
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
                _selectedFiles.Clear(); // Clear previous selections
                foreach (var file in filePickerResult)
                {
                    _selectedFiles.Add(file.FullPath);
                }
                statusLabel.Text = $"{_selectedFiles.Count} files selected.";
            }
        }

        private async void OnUploadFilesClicked(object sender, EventArgs e)
        {
            var connectionString = ConnectionString;
            var containerName = QueueName;

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
            {
                statusLabel.Text = "Please provide connection string and container name.";
                return;
            }

            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
                await containerClient.CreateIfNotExistsAsync();

                foreach (var file in _selectedFiles)
                {
                    var blobClient = containerClient.GetBlobClient(Path.GetFileName(file));
                    await blobClient.UploadAsync(file, overwrite: true);
                }

                statusLabel.Text = "Files uploaded successfully.";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
            }
        }
    }
}
