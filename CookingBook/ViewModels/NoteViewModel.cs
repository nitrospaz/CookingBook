using CommunityToolkit.Mvvm.ComponentModel;
using System;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CookingBook.ViewModels
{
    internal class NoteViewModel : ObservableObject, IQueryAttributable
    {

        private Models.Note _note;


        // The Text property first checks if the value being set is a different value.
        // If the value is different, that value is passed on to the model's property,
        // and the OnPropertyChanged method is called.

        // The OnPropertyChanged method is provided by the ObservableObject base class.
        // This method uses the name of the calling code, in this case, the property
        // name of Text, and raises the ObservableObject.PropertyChanged event.
        // This event supplies the name of the property to any event subscribers.
        // The binding system provided by.NET MAUI recognizes this event, and updates
        // any related bindings in the UI. For the Note viewmodel, when the Text property
        // changes, the event is raised, and any UI element that is bound to the Text
        // property is notified that the property changed.
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

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                _note = Models.Note.Load(query["load"].ToString());
                RefreshProperties();
            }
        }

        // The Reload method is a helper method that refreshes the backing model object,
        // reloading it from device storage
        public void Reload()
        {
            _note = Models.Note.Load(_note.Filename);
            RefreshProperties();
        }

        //The RefreshProperties method is another helper method to ensure that any
        //subscribers bound to this object are notified that the Text and Date
        //properties have changed. Since the underlying model (the _note field) is
        //changed when the note is loaded during navigation, the Text and Date
        //properties aren't actually set to new values. Since these properties aren't
        //directly set, any bindings attached to those properties wouldn't be notified
        //because OnPropertyChanged isn't called for each property. RefreshProperties
        //ensures bindings to these properties are refreshed.
        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Date));
        }

        public NoteViewModel()
        {
            _note = new Models.Note();
            SaveCommand = new AsyncRelayCommand(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        public NoteViewModel(Models.Note note)
        {
            _note = note;
            SaveCommand = new AsyncRelayCommand(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        private async Task Save()
        {
            _note.Date = DateTime.Now;
            _note.Save();
            await Shell.Current.GoToAsync($"..?saved={_note.Filename}");
        }

        private async Task Delete()
        {
            _note.Delete();
            await Shell.Current.GoToAsync($"..?deleted={_note.Filename}");
        }
    }
}
