using CommunityToolkit.Mvvm.Input;
using CookingBook.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CookingBook.Pages;

public partial class AllNotesPage : ContentPage
{
    public ObservableCollection<Note> AllNotes { get; private set; }
    public ICommand AddNewRecipeCommand { get; private set; }

    public AllNotesPage()
    {
        InitializeComponent();
        AllNotes = new ObservableCollection<Note>(Note.LoadAll());
        AddNewRecipeCommand = new RelayCommand(OnAddNewRecipe);
        BindingContext = this; // Set the BindingContext after initializing the collection
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ReloadNotes();
    }

    private void ReloadNotes()
    {
        AllNotes.Clear();
        foreach (var note in Note.LoadAll())
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
            await Shell.Current.GoToAsync($"NotePage?load={selectedNote.Filename}");
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