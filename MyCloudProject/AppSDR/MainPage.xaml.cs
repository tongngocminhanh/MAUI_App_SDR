using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class MainPage : ContentPage
    {

        /// <summary>
        /// Initialize the Binding Context for the MainViewModel() class.
        /// </summary>
        /// <param>No parameters, presents as the default page.</param>
        /// <returns>No returned values</returns>
        public MainPage()
        {
            // Default initialization for the primary logic class of MainPage.xaml
            InitializeComponent();
            // Initialize for MainViewModel class, variable Navigation
            BindingContext = new MainViewModel(Navigation);
        }
    }
}