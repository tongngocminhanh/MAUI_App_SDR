using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TestCloudProject.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BlobStorageService _blobStorageService;
        private ObservableCollection<string> _containerNames;
        private ObservableCollection<string> _blobNames;
        private string _selectedContainer;
        private string _selectedBlob;

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

        public string SelectedContainer
        {
            get { return _selectedContainer; }
            set
            {
                _selectedContainer = value;
                OnPropertyChanged();
                LoadBlobsCommand.Execute(_selectedContainer);
            }
        }

        public string SelectedBlob
        {
            get { return _selectedBlob; }
            set
            {
                _selectedBlob = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadContainersCommand { get; }
        public ICommand LoadBlobsCommand { get; }

        public MainViewModel()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=mauiprojectcloud;AccountKey=gDYct5X+8L0wUco6yIYFSvfdh/1UbwYmAAashjpETQ1czbYjS/1dtdgdhW0pjOlQoqmWqbAbXslb+AStiMasTw==;BlobEndpoint=https://mauiprojectcloud.blob.core.windows.net/;QueueEndpoint=https://mauiprojectcloud.queue.core.windows.net/;TableEndpoint=https://mauiprojectcloud.table.core.windows.net/;FileEndpoint=https://mauiprojectcloud.file.core.windows.net/;";
            _blobStorageService = new BlobStorageService(connectionString);

            LoadContainersCommand = new Command(async () => await ExecuteLoadContainersCommand());
            LoadBlobsCommand = new Command<string>(async (containerName) => await LoadBlobsAsync(containerName));
        }

        private async Task ExecuteLoadContainersCommand()
        {
            try
            {
                ContainerNames = await LoadContainersAsync();
                if (ContainerNames == null || ContainerNames.Count == 0)
                {
                    Console.WriteLine("No containers found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading containers: {ex.Message}");
            }
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
                return new ObservableCollection<string>();
            }
        }

        private async Task LoadBlobsAsync(string containerName)
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
