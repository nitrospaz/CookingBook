using CommunityToolkit.Mvvm.Input;
using CookingBook.Models;
using CookingBook.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CookingBook.Pages;

public partial class AllNotesPage : ContentPage
{
    private readonly ISqLiteService _sqLiteService;
    public ObservableCollection<Note> AllNotes { get; private set; }
    public ICommand AddNewRecipeCommand { get; private set; }

    public AllNotesPage(ISqLiteService sqLiteService)
    {
        InitializeComponent();
        _sqLiteService = sqLiteService;
        AllNotes = new ObservableCollection<Note>();
        AddNewRecipeCommand = new RelayCommand(OnAddNewRecipe);
        BindingContext = this; // Set the BindingContext after initializing the collection
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ReloadNotes();
    }

    private async void ReloadNotes()
    {
        AllNotes.Clear();
        var notes = await _sqLiteService.GetNotesAsync();
        foreach (var note in notes)
        {
            AllNotes.Add(note);
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Note selectedNote)
        {
            // Navigate to NotePage with the selected note
            //await Navigation.PushAsync(new NotePage(selectedNote));

            // Navigate to NotePage with the selected note's filename as a query parameter
            // load in current frame
            await Shell.Current.GoToAsync($"NotePage?load={selectedNote.Id}");
        }

        // Deselect the item
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void OnAddNewRecipe()
    {
        // Navigate to a new page for adding a recipe
        await Shell.Current.GoToAsync("NotePage?new=true");
    }
}