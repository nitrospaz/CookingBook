using CookingBook.Models;
using System.Text;

namespace CookingBook.Pages;

public partial class ExportRecipesPage : ContentPage
{
    public ExportRecipesPage()
    {
        InitializeComponent();
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        var recipes = Note.LoadAll();
        RecipesCollectionView.ItemsSource = recipes.Select(r => new SelectableRecipe { Recipe = r }).ToList();
    }

    private void OnSelectAllCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var selectAll = e.Value;
        var recipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().ToList();
        foreach (var recipe in recipes)
        {
            recipe.IsSelected = selectAll;
        }
        RecipesCollectionView.ItemsSource = recipes; // Refresh the CollectionView
    }

    private async void OnExportToCsvClicked(object sender, EventArgs e)
    {
        // Fetch selected recipes
        var selectedRecipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().Where(r => r.IsSelected).Select(r => r.Recipe);

        // Convert recipes to CSV format
        var csv = ConvertRecipesToCsv(selectedRecipes);

        // Get the desktop path
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var fileName = "recipes.csv";

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

public class SelectableRecipe
{
    public Note Recipe { get; set; }
    public bool IsSelected { get; set; }
}