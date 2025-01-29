using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CookingBook.Models
{
    public class Note
    {
        public string Title { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;

        public string Filename { get; set; }
        public string Text { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public Note()
        {
            // The constructor initializes the note with a random filename
            Filename = $"{Path.GetRandomFileName()}.notes.txt";
            DateModified = DateTime.Now;
            DateCreated = DateTime.Now;
            Text = "";
        }

        public void Save()
        {
            var noteData = new
            {
                Title,
                Rating,
                Description,
                Ingredients,
                Instructions,
                Comments,
                Categories,
                Tags,
                Author,
                Source,
                SourceUrl,
                Text,
                DateCreated,
                DateModified
            };

            var json = JsonSerializer.Serialize(noteData);
            File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, Filename), json);
        }

        public void Delete() =>
            File.Delete(System.IO.Path.Combine(FileSystem.AppDataDirectory, Filename));

        public static Note Load(string filename)
        {
            filename = Path.Combine(FileSystem.AppDataDirectory, filename);
            // Retry mechanism with a delay
            for (int i = 0; i < 1; i++)
            {
                bool ex = File.Exists(filename);
                if (File.Exists(filename))
                {
                    var fileContent = File.ReadAllText(filename);

                    Note noteData;
                    try
                    {
                        noteData = JsonSerializer.Deserialize<Note>(fileContent, jsonSerializerOptions) ?? new Note { Text = fileContent };
                    }
                    catch (JsonException)
                    {
                        // If deserialization fails, treat the content as plain text
                        noteData = new Note
                        {
                            Text = fileContent
                        };
                    }

                    return new Note
                    {
                        Filename = Path.GetFileName(filename),
                        Title = noteData?.Title ?? string.Empty,
                        Rating = noteData?.Rating ?? 0,
                        Description = noteData?.Description ?? string.Empty,
                        Ingredients = noteData?.Ingredients ?? string.Empty,
                        Instructions = noteData?.Instructions ?? string.Empty,
                        Comments = noteData?.Comments ?? string.Empty,
                        Categories = noteData?.Categories ?? string.Empty,
                        Tags = noteData?.Tags ?? string.Empty,
                        Author = noteData?.Author ?? string.Empty,
                        Source = noteData?.Source ?? string.Empty,
                        SourceUrl = noteData?.SourceUrl ?? string.Empty,
                        Text = noteData?.Text ?? string.Empty,
                        DateCreated = noteData?.DateCreated ?? DateTime.Now,
                        DateModified = noteData?.DateModified ?? DateTime.Now
                    };
                }
                else
                {
                    // Wait for 1 second before retrying
                    Task.Delay(1000).Wait();
                }
            }

            // If the file still cannot be found, return a default Note object
            return new Note();
        }

        public static IEnumerable<Note> LoadAll()
        {
            // Get the folder where the notes are stored.
            string appDataPath = FileSystem.AppDataDirectory;

            // Use Linq extensions to load the *.notes.txt files.
            return Directory

                    // Select the file names from the directory
                    .EnumerateFiles(appDataPath, "*.notes.txt")

                    // Each file name is used to load a note
                    .Select(filename => Note.Load(Path.GetFileName(filename)))

                    // With the final collection of notes, order them by date
                    .OrderByDescending(note => note.DateModified);
        }
    }

}
