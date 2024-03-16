namespace AppSDR
{
    public partial class TextEditorPage : ContentPage
    {
        public string[] EntryCellValues { get; set; }

        public TextEditorPage(string[] entryCellValues)
        {
            InitializeComponent();
            EntryCellValues = entryCellValues;
        }
        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Navigate back to the MainPage
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
                await Navigation.PushModalAsync(new Page1(activeCellsArray, EntryCellValues));
            }
            catch (Exception ex)
            {
                // Handle any other exceptions here
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Please input SDR");
            }
        }

    }
}