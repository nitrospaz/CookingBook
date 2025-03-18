using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CookingBook.Services;

namespace CookingBook.Pages;

public partial class NotePage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private ISqLiteService _dataAccess;
    private Models.Note _note;
    private bool _hasUnsavedChanges;
    private bool _isSaving;
    private bool isEditMode = true;

     public bool IsEditMode
    {
        get => isEditMode;
        set
        {
            if (isEditMode != value)
            {
                isEditMode = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Title
    {
        get => _note.Title;
        set
        {
            if (_note.Title != value)
            {
                _note.Title = value;
                OnPropertyChanged();
            }
        }
    }

    public string Description
    {
        get => _note.Description;
        set
        {
            if (_note.Description != value)
            {
                _note.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public string Ingredients
    {
        get => _note.Ingredients;
        set
        {
            if (_note.Ingredients != value)
            {
                _note.Ingredients = value;
                OnPropertyChanged();
            }
        }
    }

    public string Instructions
    {
        get => _note.Instructions;
        set
        {
            if (_note.Instructions != value)
            {
                _note.Instructions = value;
                OnPropertyChanged();
            }
        }
    }

    public string Text
    {
        get => _note.Text;
        set
        {
            if (_note.Text != value)
            {
                _note.Text = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateModified => _note.DateModified;

    public int Identifier => _note.Id;

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public NotePage(ISqLiteService sqLiteService)
	{
		InitializeComponent();
        _dataAccess = sqLiteService;
        _note = new Models.Note();
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);
        BindingContext = this;
        Shell.Current.Navigating += OnShellNavigating;
        ToggleEditMode(); // default to read mode
    }

    private void OnToggleEditMode(object sender, EventArgs e)
    {
        isEditMode = !isEditMode;
        UpdateViewMode();
    }

    private void ToggleEditMode()
    {
        IsEditMode = !IsEditMode;
        UpdateViewMode();
    }

    private void UpdateViewMode()
    {
        TitleEditor.IsVisible = isEditMode;
        DescriptionEditor.IsVisible = isEditMode;
        IngredientsEditor.IsVisible = isEditMode;
        InstructionsEditor.IsVisible = isEditMode;
        TextEditor.IsVisible = isEditMode;

        TitleLabel.IsVisible = !isEditMode;
        DescriptionLabel.IsVisible = !isEditMode;
        IngredientsLabel.IsVisible = !isEditMode;
        InstructionsLabel.IsVisible = !isEditMode;
        TextLabel.IsVisible = !isEditMode;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("new") && bool.TryParse(query["new"].ToString(), out bool isNew) && isNew)
        {
            if(_hasUnsavedChanges)
            {
                // do nothing.
                return;
            }
            else
            {
                // load blank note
                _note = new Models.Note();
                // toggle to edit mode from read mode
                ToggleEditMode();
            }
        }
        else if (query.ContainsKey("load") && int.TryParse(query["load"].ToString(), out int id))
        {
            //_note = Models.Note.Load(query["load"].ToString());
            _note = await _dataAccess.GetNoteAsync(id);
        }
        else
        {
            _note = new Models.Note();
        }
        RefreshProperties();
    }

    public async void Reload()
    {
        //_note = Models.Note.Load(_note.Filename);
        _note = await _dataAccess.GetNoteAsync(_note.Id);
        RefreshProperties();
    }

    public void ClearPage()
    {
        // Clear the page
        _note = new Models.Note();
        RefreshProperties();
    }

    private void RefreshProperties()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Ingredients));
        OnPropertyChanged(nameof(Instructions));
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(DateModified));
        _hasUnsavedChanges = false;
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_note.Title))
        {
            await DisplayAlert("Error", "Note Title cannot be empty.", "OK");
            return;
        }

        _isSaving = true;
        _note.DateModified = DateTime.Now;
        //_note.Save();
        await _dataAccess.SaveNoteAsync(_note);
        await Shell.Current.GoToAsync($"..?saved={_note.Id}");

        // Clear the page
        ClearPage();

        // Navigate to AllNotesPage
        // if not using the ".." syntax, the page will be pushed onto the navigation stack
        //  ".." syntax is for routes without UI like saving a note
        // if calling routes that have been defined for a floyout item in appshell.xaml,
        //  you need to use // before the URI
        // if calling routes defined in appshell.xaml.cs, NOT a flyout item, 
        //  just use the uri with no slashes. This also gives you the back arrow at the top.
        await Shell.Current.GoToAsync("//AllNotesPage");
        _isSaving = false;
    }

    private async Task Delete()
    {
        //_note.Delete();
        await _dataAccess.DeleteNoteAsync(_note);
        await Shell.Current.GoToAsync($"..?deleted={_note.Id}");

        // Clear the page
        ClearPage();

        // Navigate to AllNotesPage
        await Shell.Current.GoToAsync("//AllNotesPage");
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if(propertyName != "IsEditMode")
        {
            _hasUnsavedChanges = true;
        }
    }

    private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
    {
        // Check if the target page is NotePage and if it is a new note being created
        if (e.Target.Location.OriginalString.Contains("NotePage") && e.Target.Location.OriginalString.Contains("new=true"))
        {
            return;
        }

        // Check if the target page is NotePage and if it is a new note being created
        if (e.Target.Location.OriginalString.Contains("NotePage") && e.Target.Location.OriginalString.Contains("load="))
        {
            return;
        }

        if (_hasUnsavedChanges && !_isSaving)
        {
            e.Cancel(); // Cancel the navigation

            string action = await DisplayActionSheet("There are unsaved changes \n on this page: ", "Cancel", null, "Save", "Discard");
            if (action == "Save")
            {
                // Call the save command
                if (SaveCommand is IAsyncRelayCommand asyncSaveCommand && asyncSaveCommand.CanExecute(null))
                {
                    await asyncSaveCommand.ExecuteAsync(null);
                }
            }
            else if (action == "Discard")
            {
                // Allow navigation if the user chooses to discard changes
                _hasUnsavedChanges = false;
                await Shell.Current.GoToAsync(e.Target.Location.ToString());
            }
            // If the user chooses "Cancel", do nothing and stay on the current page
        }
    }
}