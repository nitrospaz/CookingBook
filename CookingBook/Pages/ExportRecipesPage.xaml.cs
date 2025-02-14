using CookingBook.Models;
using CookingBook.Utilities;
using System.Diagnostics;
using System.Text;

namespace CookingBook.Pages;

public partial class ExportRecipesPage : ContentPage
{
    public ExportRecipesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecipes();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ClearSelections();
    }

    private void LoadRecipes()
    {
        var recipes = Note.LoadAll();
        RecipesCollectionView.ItemsSource = recipes.Select(r => new SelectableRecipe { Recipe = r }).ToList();
    }

    private void ClearSelections()
    {
        var recipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().ToList();
        foreach (var recipe in recipes)
        {
            recipe.IsSelected = false;
        }
        RecipesCollectionView.ItemsSource = recipes; // Refresh the CollectionView
        SelectAllCheckBox.IsChecked = false; // Clear the "Select All" checkbox
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
        // TODO it works for windows, need to make it work for android
        // Fetch selected recipes
        var selectedRecipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().Where(r => r.IsSelected).Select(r => r.Recipe);

        // Convert recipes to CSV format
        var csv = ConvertRecipesToCsv(selectedRecipes);

        // Get the file path
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var fileName = "recipes.csv";
        var filePath = Path.Combine(desktopPath, fileName);

        // check to see if the file exists
        if (File.Exists(filePath))
        {
            // If the file exists, ask the user if they want to overwrite it
            var overwrite = await DisplayAlert("File Exists", $"The file {fileName} already exists. Do you want to overwrite it?", "Yes", "No");
            if (!overwrite)
            {
                return;
            }
        }

        // check to see if the file is locked
        if (FileAccessUtility.IsFileLocked(filePath))
        {
            await DisplayAlert("File In Use.", $"The file {fileName} is currently in use. Please close the file and try again.", "OK");
            return;
        }

        try
        {
            // Save the CSV file
            File.WriteAllText(filePath, csv);

            // Notify the user
            await DisplayAlert("Export Successful", $"Recipes exported to {filePath}", "OK");
        }
        catch(Exception ex)
        {
            Debug.WriteLine("Write not successful. Error: " + ex);

            // Notify the user
            await DisplayAlert("Export Failed", $"Something went wrong.", "OK");
        }

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