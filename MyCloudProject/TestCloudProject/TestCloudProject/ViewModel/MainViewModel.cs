using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TestCloudProject.Viewmodel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BlobStorageService _blobStorageService;
        private ObservableCollection<string> _containerNames;
        private ObservableCollection<string> _blobNames;

        public ObservableCollection<string> ContainerNames
        {
            get { return _containerNames; }
            set
            {
                _containerNames = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> BlobNames
        {
            get { return _blobNames; }
            set
            {
                _blobNames = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=mauiprojectcloud;AccountKey=gDYct5X+8L0wUco6yIYFSvfdh/1UbwYmAAashjpETQ1czbYjS/1dtdgdhW0pjOlQoqmWqbAbXslb+AStiMasTw==;BlobEndpoint=https://mauiprojectcloud.blob.core.windows.net/;QueueEndpoint=https://mauiprojectcloud.queue.core.windows.net/;TableEndpoint=https://mauiprojectcloud.table.core.windows.net/;FileEndpoint=https://mauiprojectcloud.file.core.windows.net/;";
            _blobStorageService = new BlobStorageService(connectionString);
        }

        public async Task<ObservableCollection<string>> LoadContainersAsync()
        {
            try
            {
                return await _blobStorageService.ListContainersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading containers: {ex.Message}");
                throw;
            }
        }

        public async Task LoadBlobsAsync(string containerName)
        {
            try
            {
                BlobNames = await _blobStorageService.ListBlobsAsync(containerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading blobs in container '{containerName}': {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
