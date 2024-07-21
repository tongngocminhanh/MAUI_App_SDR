using MauiApp1.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private QueueMessageListener _listener;
        private CancellationTokenSource _cts;
        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
          
        }

        private async void OnUploadFilesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UploadPage());
        }

        private void OnStartListeningClicked(object sender, EventArgs e)
        {
            string connectionString = ConnectionStringEntry.Text;
            string queueName = QueueNameEntry.Text;

            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName))
            {
                StatusLabel.Text = "Status: Please enter valid connection string and queue name.";
                return;
            }
            _viewModel = new MainViewModel(this.Navigation);
            _listener = new QueueMessageListener(connectionString, queueName, StatusLabel,Navigation);
         
            _cts = new CancellationTokenSource();

            Task.Run(async () => await _listener.ListenToMessagesAsync(_cts.Token));

            StatusLabel.Text = "Status: Listening...";
        }

        private void OnStopListeningClicked(object sender, EventArgs e)
        {
            _cts?.Cancel();
            StatusLabel.Text = "Status: Stopped";
        }
    }
}
