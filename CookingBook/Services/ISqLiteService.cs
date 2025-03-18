using CookingBook.Models;

namespace CookingBook.Services
{
    public interface ISqLiteService
    {
        Task<IEnumerable<Note>> GetNotesAsync();
        Task<Note> GetNoteAsync(int id);
        Task SaveNoteAsync(Note note);
        Task DeleteNoteAsync(Note note);
    }
}
