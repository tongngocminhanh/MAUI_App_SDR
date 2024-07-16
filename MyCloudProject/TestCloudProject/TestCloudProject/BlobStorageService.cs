using Azure.Storage.Blobs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public class BlobStorageService
{
    private string _connectionString;

    public BlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ObservableCollection<string>> ListContainersAsync()
    {
        ObservableCollection<string> containerNames = new ObservableCollection<string>();

        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

        try
        {
            await foreach (var container in blobServiceClient.GetBlobContainersAsync())
            {
                containerNames.Add(container.Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing containers: {ex.Message}");
            throw;
        }

        return containerNames;
    }

    public async Task<ObservableCollection<string>> ListBlobsAsync(string containerName)
    {
        ObservableCollection<string> blobNames = new ObservableCollection<string>();

        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

        try
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobNames.Add(blobItem.Name);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing blobs in container '{containerName}': {ex.Message}");
            throw;
        }

        return blobNames;
    }
}
