namespace AppSDR
{
    public partial class TextEditorPage : ContentPage
    {
        private INavigation Navigation;
        public string[] EntryCellValues { get; set; }
        public string ConnectionString { get; set; }
        public string DownloadBlobStorage { get; set; }

        public TextEditorPage(string[] entryCellValues, string connectionString, string downloadBlobStorage, INavigation navigation)
        {
            InitializeComponent();
            EntryCellValues = entryCellValues;
            ConnectionString = connectionString;
            DownloadBlobStorage = downloadBlobStorage;  
            Navigation = navigation;    
        }
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                string fileContent = textEditor.Text;
                if (string.IsNullOrEmpty(fileContent))
                {
                    // If it's empty, display an alert
                    await Application.Current.MainPage.DisplayAlert("Error", "Please type an SDR", "OK");
                    return; // Exit the method
                }
                // Split the content into lines
                string[] lines = fileContent.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);


                // Initialize a list to store the parsed rows
                List<int[]> activeCellsColumn = new List<int[]>();

                // Iterate over each line and parse the integers
                foreach (string line in lines)
                {
                    // Split each line into individual integers
                    string[] numbers = line.Split(',');

                    // Parse each number and store it in a list
                    List<int> parsedNumbers = new List<int>();

                    // Flag to indicate if the row has at least one non-zero value
                    bool hasNonZeroValue = false;

                    foreach (string numberString in numbers)
                    {
                        if (int.TryParse(numberString, out int parsedNumber))
                        {
                            // Check if it's the first value in the row
                            if (parsedNumbers.Count == 0 && parsedNumber == 0)
                            {
                                // Skip the row if the first value is 0
                                hasNonZeroValue = false;
                                break;
                            }

                            parsedNumbers.Add(parsedNumber);

                            // Set the flag if a non-zero value is found
                            if (!hasNonZeroValue && parsedNumber != 0)
                            {
                                hasNonZeroValue = true;
                            }
                        }
                        else
                        {
                            // Handle parsing error if needed
                            throw new ArgumentException("Invalid number format");
                        }
                    }

                    // Add the parsed numbers to the list if the row has at least one non-zero value
                    if (hasNonZeroValue)
                    {
                        activeCellsColumn.Add(parsedNumbers.ToArray());
                    }
                }


                // Convert the list to a 2D array
                int[][] activeCellsArray = activeCellsColumn.ToArray();

                await Application.Current.MainPage.DisplayAlert("Success", "Data saved successfully", "OK");
                await Navigation.PushModalAsync(new Page1(activeCellsArray, EntryCellValues, ConnectionString, DownloadBlobStorage, Navigation));
            }
            catch (Exception ex)
            {
                // Handle any other exceptions here
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Please input SDR");
            }
        }
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage()); // Navigate back to the MainPage
        }
        private async void OnToUploadPageClicked(object sender, EventArgs e)
        {
            try
            {
                // Get the content from the text editor
                string fileContent = textEditor.Text;
                if (string.IsNullOrEmpty(fileContent))
                {
                    // If the content is empty, display an alert
                    await Application.Current.MainPage.DisplayAlert("Error", "Please type an SDR", "OK");
                    return; // Exit the method
                }

                // Get the path to the user's desktop directory
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                // Define the file name and full path
                string fileName = "SavedSDR.txt";
                string filePath = Path.Combine(desktopPath, fileName);

                // Write the content to the file
                File.WriteAllText(filePath, fileContent);

                // Notify the user of success
                await Application.Current.MainPage.DisplayAlert("Success", "Data saved to desktop successfully", "OK");
                // Navigate to UploadPage with the file path
                await Navigation.PushModalAsync(new UploadPage(filePath, Navigation, EntryCellValues));
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the file save process
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "An error occurred while saving the file");
            }
        }
    }
}