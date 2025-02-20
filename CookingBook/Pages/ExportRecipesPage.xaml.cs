using CookingBook.Models;
using CookingBook.Services;
using CookingBook.Utilities;
using System.Diagnostics;
using System.Text;

namespace CookingBook.Pages;

public partial class ExportRecipesPage : ContentPage
{
    public ExportRecipesPage()
    {
        InitializeComponent();
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
        // TODO it works for windows, need to make it work for android
        // Fetch selected recipes
        var selectedRecipes = RecipesCollectionView.ItemsSource.Cast<SelectableRecipe>().Where(r => r.IsSelected).Select(r => r.Recipe);

        // Convert recipes to CSV format
        var csv = ConvertRecipesToCsv(selectedRecipes);

        // Get the file path
        string filePath = string.Empty;
        var fileName = "recipes.csv";

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            try
            {
                var fileService = DependencyService.Get<IFileService>();
                if (fileService == null)
                {
                    Debug.WriteLine("IFileService is not registered with DependencyService.");
                    await DisplayAlert("Export Failed", "File service is not available.", "OK");
                    return;
                }

                if (csv == null)
                {
                    Debug.WriteLine("CSV content is null.");
                    await DisplayAlert("Export Failed", "CSV content is null.", "OK");
                    return;
                }

                // Save the CSV file
                await fileService.CreateCsvFileAsync("recipes.csv", csv);

                // Notify the user
                await DisplayAlert("Export Successful", "CSV file saved to Downloads folder.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occurred while creating CSV file: " + ex);
                await DisplayAlert("Export Failed", $"An error occurred: {ex.Message}", "OK");
            }

            return;
        }
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            // For Windows, save the file in the Documents folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filePath = Path.Combine(documentsPath, fileName);
        }
        else
        {
            // For other platforms, save the file in the Documents folder
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filePath = Path.Combine(desktopPath, fileName);
        }

        // check to see if the file exists
        if (File.Exists(filePath))
        {
            // If the file exists, ask the user if they want to overwrite it
            var overwrite = await DisplayAlert("File Exists", $"The file {fileName} already exists. Do you want to overwrite it?", "Yes", "No");
            if (!overwrite)
            {
                // If the user does not want to overwrite the file, return
                return;
            }
            else
            {
                // if they want to overwrite, check to see if the file is locked
                if (FileAccessUtility.IsFileLocked(filePath))
                {
                    await DisplayAlert("File In Use.", $"The file {fileName} is currently in use. Please close the file and try again.", "OK");
                    return;
                }
            }
        }
        else
        {
            // If the file does not exist, check to see if the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                try
                {
                    // If the directory does not exist, create it
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Directory not created. Error: " + ex);
                    await DisplayAlert("Export Failed", $"Something went wrong. Directory not available.", "OK");
                    return;
                }
            }
        }

        try
        {
            // Save the CSV file with UTF-8 encoding
            File.WriteAllText(filePath, csv, Encoding.UTF8);

            // Notify the user
            await DisplayAlert("Export Successful", $"Recipes exported to {filePath}", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Write not successful. Error: " + ex);

            // Notify the user
            await DisplayAlert("Export Failed", $"Error trying to write the file.", "OK");
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