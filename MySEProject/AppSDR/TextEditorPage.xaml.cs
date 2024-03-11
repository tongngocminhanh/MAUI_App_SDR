using Microsoft.Maui.Controls;

namespace AppSDR;

    public partial class TextEditorPage : ContentPage
    {
        public string[] EntryCellValues { get; set; }
        int[][] activeCellsColumn { get; set; }
        public TextEditorPage(string[] entryCellValues)
        {
            InitializeComponent();
            EntryCellValues = entryCellValues;
        }

        private void OnEditorCompleted(object sender, EventArgs e)
        {
            // Insert a newline character when Enter is pressed
            textEditor.Text += Environment.NewLine;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Get the entered text
            string enteredText = textEditor.Text;

            // Split the entered text by lines
            string[] lines = enteredText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Initialize a list to store the parsed rows
            List<int[]> rows = new List<int[]>();

            // Parse each line and split by commas
            foreach (string line in lines)
            {
                // Split the line by commas and parse each number
                int[] row = line.Split(',')
                                 .Select(str =>
                                 {
                                     if (int.TryParse(str.Trim(), out int num))
                                         return num;
                                     else
                                         return 0; // or handle the parsing error accordingly
                                 })
                                 .ToArray();

                // Add the parsed row to the list
                rows.Add(row);
            }

            // Convert the list of rows to a 2D array
            activeCellsColumn = rows.ToArray();

            // Close the text editor page
            await Navigation.PopAsync();
            await Navigation.PushAsync(new Page1(activeCellsColumn, EntryCellValues));
        }
    }
