using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;
using System.Text.Json;
using Azure.Data.Tables;
using Azure;

namespace AppSDR
{
    public class QueueMessageListener
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private INavigation _navigation;

        /// <summary>
        /// Constructor for the QueueMessageListener class.
        /// Initializes the listener with configuration parameters for accessing Azure Queue Storage and navigation instance.
        /// </summary>
        /// <param name="messageConfig">An array containing a connection string and a the queue name.</param>
        /// <param name="navigation">The navigation instance for handling page navigation.</param>
        /// <returns>
        /// Initializes private fields with connection string, queue name, and navigation instance for 
        /// listening to and processing messages from the queue.
        /// </returns>
        public QueueMessageListener(string[] messageConfig, INavigation navigation)
        {
            _connectionString = messageConfig[0];
            _queueName = messageConfig[1];
            _navigation = navigation;
        }

        /// <summary>
        /// Asynchronously listens to messages from the Azure Queue Storage and processes them.
        /// Continuously polls the queue for new messages, handles base64 encoded messages, 
        /// deserializes JSON data, processes experiment requests, and handles errors.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to signal the operation to stop if needed.</param>
        /// <returns>Processes messages from the queue, handling each message, deleting processed messages from the queue.</returns>
        public async Task ListenToMessagesAsync(CancellationToken cancellationToken)
        {
            QueueClient queueClient = new QueueClient(_connectionString, _queueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (await queueClient.ExistsAsync())
                    {
                        QueueMessage[] receivedMessages = await queueClient.ReceiveMessagesAsync(maxMessages: 10, visibilityTimeout: TimeSpan.FromSeconds(30), cancellationToken: cancellationToken);

                        // Handle JSON data
                        foreach (var message in receivedMessages)
                        {
                            string messageText = message.Body.ToString();

                            if (IsBase64String(messageText))
                            {
                                byte[] data = Convert.FromBase64String(messageText);
                                messageText = Encoding.UTF8.GetString(data);
                            }

                            ExperimentRequestMessage experimentRequestMessage = null;
                            try
                            {
                                experimentRequestMessage = JsonSerializer.Deserialize<ExperimentRequestMessage>(messageText);
                            }
                            catch (JsonException ex)
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    Application.Current.MainPage.DisplayAlert("Error", $"JSON Deserialization Error: {ex.Message}", "OK");
                                });
                            }

                            // Operate only when MESSAGE has content
                            if (experimentRequestMessage != null)
                            {
                                await ProcessExperimentRequestAsync(experimentRequestMessage);
                            }

                            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                        }
                    }

                    await Task.Delay(5000, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving messages: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage.DisplayAlert("Error", $"Fail to receive messages: {ex.Message}", "OK");
                    });
                }
            }
        }

        /// <summary>
        /// Checks if a string is a valid Base64 encoded string.
        /// </summary>
        /// <param name="base64">The string to be checked.</param>
        /// <returns>True if the string is a valid Base64 encoded string; otherwise, false.</returns>
        private bool IsBase64String(string base64)
        {
            // Handle JSON data
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        /// <summary>
        /// Processes an experiment request by downloading blobs from Azure Blob Storage and performing further operations on the downloaded files.
        /// Handles errors during blob operations and provides user feedback via alerts.
        /// </summary>
        /// <param name="request">An instance of ExperimentRequestMessage containing details for accessing blob storage and processing files.</param>
        /// <returns>Processes each blob in the specified container, downloads it to the desktop, and calls other functions</returns>
        private async Task ProcessExperimentRequestAsync(ExperimentRequestMessage request)
        {
            try
            {
                // Use ExperimentRequestMessage,containing Storage Account information, access to it
                BlobServiceClient blobServiceClient = new BlobServiceClient(request.StorageConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(request.UploadBlobStorageName);

                if (!await containerClient.ExistsAsync())
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "Container does not exist.", "OK");
                    });
                    return;
                }

                // Run through all files in Blob
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
                        // Generating functions
                        await DownloadBlobToFileAsync(blobClient, localFilePath);
                        await ProcessDownloadedFileAsync(localFilePath, request);
                    }
                    catch (Exception ex)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Application.Current.MainPage.DisplayAlert("Error", $"Error downloading or processing blob '{blobName}': {ex.Message}", "OK");
                        });
                    }
                }

                Console.WriteLine($"All files have been processed.");
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Error", $"Error processing experiment request: {ex.Message}", "OK");
                });
            }
        }

        /// <summary>
        /// Downloads a blob from Azure Blob Storage to a specified file path on the local filesystem.
        /// Handles errors during the download process and provides user feedback via alerts.
        /// </summary>
        /// <param name="blobClient">The BlobClient instance used to download the blob.</param>
        /// <param name="filePath">The local file path where the blob will be saved.</param>
        /// <returns>Downloads the blob to the specified file path and handles errors during the process.</returns>
        private async Task DownloadBlobToFileAsync(BlobClient blobClient, string filePath)
        {
            try
            {
                // Download SDR files to Desktop
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await download.Content.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Error", $"Error downloading blob to file {filePath}: {ex.Message}", "OK");
                });
                throw;
            }
        }

        /// <summary>
        /// Sanitizes a file name to ensure it is valid for use in the filesystem.
        /// Replaces invalid characters with underscores.
        /// </summary>
        /// <param name="fileName">The original file name to be sanitized.</param>
        /// <returns>A sanitized file name where invalid characters have been replaced with underscores.</returns>
        private string SanitizeFileName(string fileName)
        {
            // Make the file names valid if needed
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        /// <summary>
        /// Processes a downloaded file by reading its content, parsing it, and creating a new Page1 instance with the parsed data.
        /// Handles errors during file processing and provides user feedback via alerts.
        /// </summary>
        /// <param name="filePath">The path to the file that has been downloaded and needs to be processed.</param>
        /// <param name="request">An instance of ExperimentRequestMessage containing details for processing the file.</param>
        /// <returns>Reads and parses the file content, creates a new Page1 instance, and navigates to it, saves a screenshot to blob storage.</returns>
        private async Task ProcessDownloadedFileAsync(string filePath, ExperimentRequestMessage request)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(filePath);

                // Assign SDR values, and drawing parameters
                int[][] activeCellsArray = ParseFileContent(fileContent);
                string[] entryCellValues = await DownloadEntity(_connectionString, request.TableStorageName);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    string[] _cloudConfig = new string[] { _connectionString, request.DownloadBlobStorageName };
                    var page1FromUploadPage = new Page1(activeCellsArray, entryCellValues, _cloudConfig, _navigation, typeof(UploadPage));
                    await _navigation.PushModalAsync(page1FromUploadPage);

                    await page1FromUploadPage.SaveScreenshotToBlobStorage();
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Error", $"Error processing downloaded file {filePath}: {ex.Message}", "OK");
                });
            }
        }

        /// <summary>
        /// Downloads entity data from Azure Table Storage based on the provided connection string and table storage name.
        /// </summary>
        /// <param name="ConnectionString">The connection string for accessing Azure Table Storage.</param>
        /// <param name="TableStorageName">The name of the table in Azure Table Storage from which to download data.</param>
        /// <returns>An array of strings representing the entry cell values retrieved from the most recent entity in the table.</returns>
        private async Task<string[]> DownloadEntity(string ConnectionString, string TableStorageName)
        {
            try
            {
                // Access to Table Storage and assign EntryCellValues
                TableClient tableClient = new TableClient(ConnectionString, TableStorageName);
                Pageable<TableEntityConfiguration> queryResults = tableClient.Query<TableEntityConfiguration>();
                List<TableEntityConfiguration> entities = queryResults.ToList();
                var mostRecentEntity = entities.OrderByDescending(ent => ent.Timestamp).FirstOrDefault();

                if (mostRecentEntity != null)
                {
                    return new string[]
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
                }
                else
                {
                    throw new InvalidOperationException("No entities found in the table.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int[][] ParseFileContent(string fileContent)
        {
            // Similar class to ParseFileContent() in MainViewModel()
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
                    }
                }

                if (hasNonZeroValue)
                {
                    activeCellsColumn.Add(parsedNumbers.ToArray());
                }
            }
            return activeCellsColumn.ToArray();
        }
    }
}
