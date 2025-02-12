using CookingBook.Models;
using System.Text;

namespace CookingBook.Pages;

public partial class ExportRecipesPage : ContentPage
{
	public ExportRecipesPage()
	{
		InitializeComponent();
	}

    private async void OnExportToCsvClicked(object sender, EventArgs e)
    {
        // Fetch recipes from your data source
        var recipes = Note.LoadAll();

        // Convert recipes to CSV format
        var csv = ConvertRecipesToCsv(recipes);

        //// Save the CSV file
        //var filePath = Path.Combine(FileSystem.AppDataDirectory, "recipes.csv");
        //File.WriteAllText(filePath, csv);

        // Get the desktop path
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var fileName = "recipes.csv";

        // Check if the checkbox is checked
        if (DateTimeStampCheckBox.IsChecked == true)
        {
            // Get the current date and time
            var dateTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            fileName = $"recipes_{dateTimeStamp}.csv";
        }

        var filePath = Path.Combine(desktopPath, fileName);

        // Save the CSV file
        File.WriteAllText(filePath, csv);

        // Notify the user
        await DisplayAlert("Export Successful", $"Recipes exported to {filePath}", "OK");
    }

    private string ConvertRecipesToCsv(IEnumerable<Note> recipes)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Title,Rating,Description,Ingredients,Instructions,Comments,Categories,Tags,Author,Source,SourceUrl,DateCreated,DateModified");

        foreach (var recipe in recipes)
        {
            sb.AppendLine($"{EscapeCsvField(recipe.Title)},{recipe.Rating},{EscapeCsvField(recipe.Description)},{EscapeCsvField(recipe.Ingredients)},{EscapeCsvField(recipe.Instructions)},{EscapeCsvField(recipe.Comments)},{EscapeCsvField(recipe.Categories)},{EscapeCsvField(recipe.Tags)},{EscapeCsvField(recipe.Author)},{EscapeCsvField(recipe.Source)},{EscapeCsvField(recipe.SourceUrl)},{recipe.DateCreated},{recipe.DateModified}");
        }

        return sb.ToString();
    }

    private string EscapeCsvField(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }
        return field;
    }
}