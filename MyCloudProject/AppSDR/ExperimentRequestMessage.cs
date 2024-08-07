namespace AppSDR
{
    public class ExperimentRequestMessage
    {
        public string StorageConnectionString { get; set; }
        public string UploadBlobStorageName { get; set; }
        public string DownloadBlobStorageName {  get; set; }
        public string TableStorageName { get; set; }
    }
}
