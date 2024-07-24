using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        public UploadPage()
        {
            InitializeComponent();
            var viewModel = new UploadViewModel();
            BindingContext = viewModel;
        }
    }
}