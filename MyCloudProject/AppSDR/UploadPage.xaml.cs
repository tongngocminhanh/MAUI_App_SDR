using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        private string AssignedTextFilePath { get; set; }
        private INavigation Navigation { get; set; }
        private string[] EntryCellValue { get; set; }
        private string[] MessageConfig { get; set; }
        public UploadPage(string assignedTextFilePath, string[] messageConfig, INavigation navigation, string[] entryCellValue)
        {
            AssignedTextFilePath = assignedTextFilePath;
            MessageConfig = messageConfig;
            Navigation = navigation;
            EntryCellValue = entryCellValue;

            InitializeComponent();
            BindingContext = new UploadViewModel(AssignedTextFilePath, MessageConfig, Navigation, EntryCellValue);
        }
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage()); // Navigate back to the MainPage
        }
    }
}