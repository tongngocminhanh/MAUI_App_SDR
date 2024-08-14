using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        private string AssignedTextFilePath { get; set; }
        private INavigation Navigation { get; set; }
        private string[] EntryCellValue { get; set; }
        private string[] MessageConfig { get; set; }

        /// <summary>
        /// Initializes a new instance of the UploadPage class with the specified parameters.
        /// </summary>
        /// <param name="assignedTextFilePath">The file path of the assigned text file.</param>
        /// <param name="messageConfig">The message configuration settings.</param>
        /// <param name="navigation">The navigation instance for page transitions.</param>
        /// <param name="entryCellValue">The entry cell values for the upload process.</param>
        /// <return>Calls the UploadViewModel() to handle logic implementation.</return>
        public UploadPage(string assignedTextFilePath, string[] messageConfig, INavigation navigation, string[] entryCellValue)
        {
            // Variables added to UploadPage, some can be null
            AssignedTextFilePath = assignedTextFilePath;
            MessageConfig = messageConfig;
            Navigation = navigation;
            EntryCellValue = entryCellValue;

            InitializeComponent();
            //Initialize to work with binding context
            BindingContext = new UploadViewModel(AssignedTextFilePath, MessageConfig, Navigation, EntryCellValue);
        }

        /// <summary>
        /// Event handler for navigating back to the MainPage when the user clicks the "Back" button.
        /// </summary>
        /// <param name="sender">The source of the event, typically a button.</param>
        /// <param name="e">The event arguments.</param>
        /// <return>Move from Upload Page to Main Page.</return>
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage()); // Navigate back to the MainPage
        }
    }
}