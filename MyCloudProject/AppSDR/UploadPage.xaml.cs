using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class UploadPage : ContentPage
    {
        public string AssignedTextFilePath { get; set; }
        public INavigation Navigation { get; set; }
        public string[] EntryCellValue { get; set; }
        public UploadPage(string _assignedTextFilePath, INavigation _navigation, string[] _entryCellValue)
        {
            AssignedTextFilePath = _assignedTextFilePath;
            Navigation = _navigation;
            EntryCellValue = _entryCellValue;

            InitializeComponent();
            BindingContext = new UploadViewModel(AssignedTextFilePath, Navigation, EntryCellValue);
        }
    }
}