using ClassLibrary1;
using ClassLibrary1.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingBook.Services
{
    internal class SqLiteService : ISqLiteService
    {
        private readonly NoteContext _context;

        public SqLiteService()
        {
            _context = new NoteContext();
            _context.EnsureDatabaseCreated();
        }

        public async Task<IEnumerable<Note>> GetNotesAsync()
        {
            return await _context.Notes.ToListAsync();
        }

        public async Task<Note> GetNoteAsync(int id)
        {
            return await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task SaveNoteAsync(Note note)
        {
            if (note.Id == 0)
            {
                _context.Notes.Add(note);
            }
            else
            {
                _context.Notes.Update(note);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(Note note)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}
