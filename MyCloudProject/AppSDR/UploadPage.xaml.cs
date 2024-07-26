using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        public string AssignedTextFilePath { get; set; }
        public UploadPage(string _assignedTextFilePath)
        {
            AssignedTextFilePath = _assignedTextFilePath;
            InitializeComponent();
            var viewModel = new UploadViewModel(AssignedTextFilePath);
            BindingContext = viewModel;
        }
    }
}