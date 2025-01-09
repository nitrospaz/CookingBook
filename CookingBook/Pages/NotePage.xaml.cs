using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CookingBook.Pages;

public partial class NotePage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private Models.Note _note;

    public event PropertyChangedEventHandler PropertyChanged;

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

    public DateTime Date => _note.Date;

    public string Identifier => _note.Filename;

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    public NotePage()
	{
		InitializeComponent();
        _note = new Models.Note();
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);
        BindingContext = this;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("load"))
        {
            _note = Models.Note.Load(query["load"].ToString());
            RefreshProperties();
        }
    }

    public void Reload()
    {
        _note = Models.Note.Load(_note.Filename);
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
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(Date));
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_note.Text))
        {
            await DisplayAlert("Error", "Note text cannot be empty.", "OK");
            return;
        }

        _note.Date = DateTime.Now;
        _note.Save();
        await Shell.Current.GoToAsync($"..?saved={_note.Filename}");

        // Clear the page
        ClearPage();

        // Navigate to AllNotesPage
        await Shell.Current.GoToAsync("//AllNotesPage");
    }

    private async Task Delete()
    {
        _note.Delete();
        await Shell.Current.GoToAsync($"..?deleted={_note.Filename}");

        // Clear the page
        ClearPage();

        // Navigate to AllNotesPage
        await Shell.Current.GoToAsync("//AllNotesPage");
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}