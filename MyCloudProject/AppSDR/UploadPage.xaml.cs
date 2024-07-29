using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        public string AssignedTextFilePath { get; set; }
        public INavigation Navigation { get; set; }
        public UploadPage(string _assignedTextFilePath, INavigation _navigation)
        {
            AssignedTextFilePath = _assignedTextFilePath;
            Navigation = _navigation;

            InitializeComponent();
            var viewModel = new UploadViewModel(AssignedTextFilePath, Navigation);
            BindingContext = viewModel;
        }
    }
}