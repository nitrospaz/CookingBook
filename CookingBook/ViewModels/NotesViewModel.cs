using CommunityToolkit.Mvvm.Input;
using CookingBook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookingBook.ViewModels
{
    internal class NotesViewModel : IQueryAttributable
    {
        public ObservableCollection<ViewModels.NoteViewModel> AllNotes { get; }
        public ICommand NewCommand { get; }
        public ICommand SelectNoteCommand { get; }

        public NotesViewModel()
        {
            AllNotes = new ObservableCollection<ViewModels.NoteViewModel>(Models.Note.LoadAll().Select(n => new NoteViewModel(n)));
            NewCommand = new AsyncRelayCommand(NewNoteAsync);
            SelectNoteCommand = new AsyncRelayCommand<ViewModels.NoteViewModel>(SelectNoteAsync);
        }

        // Notice that the NewNoteAsync method doesn't take a parameter while the
        // SelectNoteAsync does. Commands can optionally have a single parameter
        // that is provided when the command is invoked. For the SelectNoteAsync
        // method, the parameter represents the note that's being selected.
        private async Task NewNoteAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.NotePage));
        }

        private async Task SelectNoteAsync(ViewModels.NoteViewModel note)
        {
            if (note != null)
                await Shell.Current.GoToAsync($"{nameof(Views.NotePage)}?load={note.Identifier}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("deleted"))
            {
                string noteId = query["deleted"].ToString();
                NoteViewModel matchedNote = AllNotes.Where((n) => n.Identifier == noteId).FirstOrDefault();

                // If note exists, delete it
                if (matchedNote != null)
                    AllNotes.Remove(matchedNote);
            }
            else if (query.ContainsKey("saved"))
            {
                string noteId = query["saved"].ToString();
                NoteViewModel matchedNote = AllNotes.Where((n) => n.Identifier == noteId).FirstOrDefault();

                // If note is found, update it
                if (matchedNote != null)
                {
                    matchedNote.Reload();
                    // one downside to using an ObservableCollection
                    // is that it must be manually sorted
                    // when the underlying data changes
                    // in this case, we're moving the updated note to the top
                    AllNotes.Move(AllNotes.IndexOf(matchedNote), 0);
                }
                // If note isn't found, it's new; add it.
                else
                {
                    // AllNotes.Add appends the new note to the end of the list
                    // Insert the new note at the top of the list
                    AllNotes.Insert(0, new NoteViewModel(Models.Note.Load(noteId)));
                }
            }
        }
    }
}
