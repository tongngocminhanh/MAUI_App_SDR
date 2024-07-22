using AppSDR.ViewModel;

namespace AppSDR
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(Navigation);
        }
    }
}