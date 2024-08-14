namespace AppSDR
{
    public partial class TextEditorPage : ContentPage
    {
        private INavigation _navigation;
        private string[] _entryCellValues;
        private string[] _cloudConfig;
        private string[] _messageConfig;

        /// <summary>
        /// Constructor for the TextEditorPage.
        /// Initializes the page with provided entry cell values, cloud configuration, message configuration, and navigation instance.
        /// </summary>
        /// <param name="EntryCellValues">An array of cell values entered by the user.</param>
        /// <param name="CloudConfig">An array containing cloud configuration information.</param>
        /// <param name="MessageConfig">An array containing message configuration information.</param>
        /// <param name="Navigation">The navigation instance for handling page navigation.</param>
        /// <return>Parsing the provided parameters to the private one of Text Editor Page</return>
        public TextEditorPage(string[] EntryCellValues, string[] CloudConfig, string[] MessageConfig, INavigation Navigation)
        {
            InitializeComponent();
            _cloudConfig = CloudConfig;
            _entryCellValues = EntryCellValues;
            _messageConfig = MessageConfig;
            _navigation = Navigation;    
        }

        /// <summary>
        /// Handles the "Save" button click event. 
        /// Validates and parses the content entered in the text editor, converting it to a 2D array of integers.
        /// If the input is valid, it navigates to Page1.
        /// </summary>
        /// <param name="sender">The button triggering the event.</param>
        /// <param name="e">Event arguments for the click event.</param>
        /// <return>Pushes the handled data to Page 1, waits for output.</return>
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
                var page1FromTextEditorPage = new Page1(activeCellsArray, _entryCellValues, _cloudConfig, _navigation, typeof(TextEditorPage));
                await Navigation.PushModalAsync(page1FromTextEditorPage);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions here
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Please input SDR");
            }
        }

        /// <summary>
        /// Handles the click event to navigate back to the main page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>Navigates from Text Editor Page back to Main Page.</returns>
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage()); // Navigate back to the MainPage
        }

        /// <summary>
        /// Handles the click event for saving the content to a file and navigating to the Upload Page.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>Navigates from Text Editor Page back to Upload Page.</returns>
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
                await Navigation.PushModalAsync(new UploadPage(filePath, _messageConfig, _navigation, _entryCellValues));
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the file save process
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "An error occurred while saving the file");
            }
        }
    }
}