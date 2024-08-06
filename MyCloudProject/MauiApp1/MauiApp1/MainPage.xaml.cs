using Azure.Storage.Queues;
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
            string containerName = ContainerName.Text;

            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(queueName) || string.IsNullOrWhiteSpace(containerName))
            {
                StatusLabel.Text = "Status: Please enter valid connection string and queue name.";
                return;
            }
            //_viewModel = new MainViewModel(this.Navigation);
            _listener = new QueueMessageListener(connectionString, queueName, containerName, StatusLabel,Navigation);
         
            _cts = new CancellationTokenSource();

            Task.Run(async () => await _listener.ListenToMessagesAsync(_cts.Token));

            StatusLabel.Text = "Status: Listening...";
        }

        private void OnStopListeningClicked(object sender, EventArgs e)
        {
            _cts?.Cancel();
            StatusLabel.Text = "Status: Stopped";
        }
        private async void OnSendMessageButtonClicked(object sender, EventArgs e)
        {
            string message = MessageEditor.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                await DisplayAlert("Error", "Please enter a message.", "OK");
                return;
            }

            await SendMessageToQueue(message);
        }

        private async Task SendMessageToQueue(string message)
        {
            // Retrieve the connection string for use with the application.
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=mauiprojectcloud;AccountKey=gDYct5X+8L0wUco6yIYFSvfdh/1UbwYmAAashjpETQ1czbYjS/1dtdgdhW0pjOlQoqmWqbAbXslb+AStiMasTw==;BlobEndpoint=https://mauiprojectcloud.blob.core.windows.net/;QueueEndpoint=https://mauiprojectcloud.queue.core.windows.net/;TableEndpoint=https://mauiprojectcloud.table.core.windows.net/;FileEndpoint=https://mauiprojectcloud.file.core.windows.net/;";
            string queueName = "trigger";

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            try
            {
                // Create the queue if it doesn't already exist
                await queueClient.CreateIfNotExistsAsync();

                if (await queueClient.ExistsAsync())
                {
                    // Send a message to the queue
                    await queueClient.SendMessageAsync(message);
                    await DisplayAlert("Success", "Message sent to queue", "OK");
                    MessageEditor.Text = string.Empty; // Clear the Entry after sending the message
                }
                else
                {
                    await DisplayAlert("Error", "Queue does not exist", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
            }
        }
    }
}
