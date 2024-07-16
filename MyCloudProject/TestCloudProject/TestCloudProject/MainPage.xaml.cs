using TestCloudProject.Viewmodel;

namespace TestCloudProject
{
    public partial class MainPage : ContentPage
    {
        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            BindingContext = _viewModel;

            // Load containers and blobs
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _viewModel.ContainerNames = await _viewModel.LoadContainersAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading containers: {ex.Message}");
            }
        }
    }
}