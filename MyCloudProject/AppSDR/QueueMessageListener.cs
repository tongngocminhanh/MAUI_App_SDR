using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;
using System.Text.Json;

namespace AppSDR
{
    public class QueueMessageListener
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly string _downloadBlobStorage;
        private INavigation _navigation;


        public QueueMessageListener(string connectionString, string queueName, string downloadBlobStorage,  INavigation navigation)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            _navigation = navigation;
            _downloadBlobStorage = downloadBlobStorage;
        }

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

                        foreach (var message in receivedMessages)
                        {
                            string messageText = message.Body.ToString();

                            if (IsBase64String(messageText))
                            {
                                byte[] data = Convert.FromBase64String(messageText);
                                messageText = Encoding.UTF8.GetString(data);
                            }

                            Console.WriteLine($"Message Content: {messageText}");

                            ExperimentRequestMessage experimentRequestMessage = null;
                            try
                            {
                                experimentRequestMessage = JsonSerializer.Deserialize<ExperimentRequestMessage>(messageText);
                            }
                            catch (JsonException ex)
                            {
                                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                                //UpdateStatusLabel($"JSON Deserialization Error: {ex.Message}");
                            }

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
                    //UpdateStatusLabel($"Error receiving messages: {ex.Message}");
                    Console.WriteLine($"Error receiving messages: {ex.Message}");
                }
            }
        }

        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        private async Task ProcessExperimentRequestAsync(ExperimentRequestMessage request)
        {
            try
            {
                //UpdateStatusLabel($"Processing experiment request: {request.downloadBlobStorage}");

                BlobServiceClient blobServiceClient = new BlobServiceClient(request.StorageConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(request.BlobStorageName);

                if (!await containerClient.ExistsAsync())
                {
                    //UpdateStatusLabel("Container does not exist.");
                    return;
                }

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
                        //UpdateStatusLabel($"Downloaded file to {localFilePath}");
                        Console.WriteLine($"Downloaded file to {localFilePath}");
                        await ProcessDownloadedFileAsync(localFilePath);


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

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        private async Task ProcessDownloadedFileAsync(string filePath)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(filePath);
                //UpdateStatusLabel($"Processing file: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Processing file: {Path.GetFileName(filePath)}");

                int[][] activeCellsArray = ParseFileContent(fileContent);
                string[] entryCellValues = { "GraphName", "100", "1", "XaxisTitle", "YaxisTitle", "1", "5000", "SDR" };

                // Ensure the navigation to Page1 is awaited and on the main thread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var page1 = new Page1(activeCellsArray, entryCellValues, _connectionString, _downloadBlobStorage, _navigation);
                    await _navigation.PushModalAsync(page1);

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
