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

        public QueueMessageListener(string[] messageConfig, INavigation navigation)
        {
            _connectionString = messageConfig[0];
            _queueName = messageConfig[1];
            _navigation = navigation;
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

        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        private async Task ProcessExperimentRequestAsync(ExperimentRequestMessage request)
        {
            try
            {
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
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Error", $"Error downloading blob to file {filePath}: {ex.Message}", "OK");
                });
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

        private async Task ProcessDownloadedFileAsync(string filePath, ExperimentRequestMessage request)
        {
            try
            {
                string fileContent = await File.ReadAllTextAsync(filePath);

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

        private async Task<string[]> DownloadEntity(string ConnectionString, string TableStorageName)
        {
            try
            {
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
