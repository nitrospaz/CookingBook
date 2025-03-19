using ClassLibrary1.Models;
using CookingBook.Services;
using CookingBook.Utilities;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Text;

namespace CookingBook.Pages;

public partial class ExportRecipesPage : ContentPage
{
    private readonly ISqLiteService _dataAccess;
    private readonly AlertService _alertService;

    public ExportRecipesPage(ISqLiteService sqLiteService)
    {
        InitializeComponent();
        _dataAccess = sqLiteService;
        _alertService = new AlertService(this);
        SetSaveLocationLabel();
    }

    private void SetSaveLocationLabel()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            SaveLocationLabel.Text = "Save to Downloads Folder";
        }
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            SaveLocationLabel.Text = "Save to My Documents Folder";
        }
        else
        {
            SaveLocationLabel.Text = "Save to Documents Folder";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadRecipes();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ClearSelections();
    }

    private async Task LoadRecipes()
    {
        var recipes = await _dataAccess.GetNotesAsync();
        RecipesCollectionView.ItemsSource = recipes.Select(r => new SelectableRecipe { Recipe = r }).ToList();
    }

    private void ClearSelections()
    {
        if (RecipesCollectionView.ItemsSource == null)
            return;

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
        // Fetch selected recipes
        var selectedRecipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().Where(r => r.IsSelected).Select(r => r.Recipe);

        // Convert recipes to CSV format
        var csv = ConvertRecipesToCsv(selectedRecipes);
        if ((DeviceInfo.Platform == DevicePlatform.Android) || (DeviceInfo.Platform == DevicePlatform.WinUI))
        {
            if (string.IsNullOrEmpty(csv))
            {
                Debug.WriteLine("CSV content is null or empty.");
                await _alertService.DisplayAlert("Export Failed", "CSV content is null or empty.", "OK");
                return;
            }

            var fileName = "recipes.csv";
            var fileService = DependencyService.Get<IFileService>();

            if (fileService == null)
            {
                Debug.WriteLine("IFileService is not registered with DependencyService.");
                await _alertService.DisplayAlert("Export Failed", "File service is not available.", "OK");
                return;
            }

            try
            {
                // Save the CSV file
                await fileService.CreateCsvFileAsync(fileName, csv);

                // Notify the user
                await _alertService.DisplayAlert("Export Successful", "CSV file saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occurred while creating CSV file: " + ex);
                await _alertService.DisplayAlert("Export Failed", $"An error occurred: {ex.Message}", "OK");
            }
        }
        else
        {
            Debug.WriteLine("You havent implemented and tested this feature for the other platforms yet.");
            await DisplayAlert("Export Failed", $"This feature not available for this platform yet.", "OK");
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
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r") || field.Contains("\t"))
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