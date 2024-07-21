using Azure.Storage.Blobs;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class UploadPage : ContentPage
    {
        private List<string> _selectedFiles;

        public UploadPage()
        {
            InitializeComponent();
            _selectedFiles = new List<string>();

        }

        private async void OnSelectFilesClicked(object sender, EventArgs e)
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
            // Implement file picker logic here
            var filePickerResult = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Select files to upload"
            });

            if (filePickerResult != null)
            {
                foreach (var file in filePickerResult)
                {
                    _selectedFiles.Add(file.FullPath);
                }
                statusLabel.Text = $"{_selectedFiles.Count} files selected.";
            }
        }

        private async void OnUploadFilesClicked(object sender, EventArgs e)
        {
            var connectionString = connectionStringEntry.Text;
            var containerName = uploadContainerNameEntry.Text;

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
