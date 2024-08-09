using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            // Initialize for MainViewModel class, variable Navigation
            InitializeComponent();
            BindingContext = new MainViewModel(Navigation);
        }
    }
}