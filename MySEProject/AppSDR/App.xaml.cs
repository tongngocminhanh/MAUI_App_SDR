namespace AppSDR
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Set MainPage is the first page when launching App SDR
            // Enable Navigation between Pages from MainPage
            MainPage = new NavigationPage (new MainPage());
        }
    }
}