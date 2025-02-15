using CookingBook.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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
            // The information to be saved by the Save method comes from the properties of the Note class instance.
            // These properties (Title, Rating, Description, etc.) are set elsewhere in the application before the Save method is called.
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
            string filePath = Path.Combine(FileSystem.AppDataDirectory, Filename);

            // check if file exists
            if(!File.Exists(filePath))
            {
                // If the file does not exist, check to see if the directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    try
                    {
                        // If the directory does not exist, try to create it
                        Directory.CreateDirectory(directory);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Directory not created. Error: " + ex);
                        return;
                    }
                }
                // if directory exists, continue to save the file...
            }
            else
            {
                // the file exists
                int maxRetries = 3;
                int delay = 2000; // 2 seconds

                // loop to check if the file is locked
                for (int i = 0; i < maxRetries; i++)
                {
                    if (!FileAccessUtility.IsFileLocked(filePath))
                    {
                        // file not locked, break the loop and save
                        break;
                    }
                    else
                    {
                        // file is locked, 
                        // if we havent reached the max retries
                        if (!(i == maxRetries - 1))
                        {
                            // delay for time, then retry save
                            Task.Delay(delay).Wait();
                        }
                        else
                        {
                            // If the file is still locked after the max retries,
                            // append *NEW* to the title and save to a new file
                            Title += " *NEW*";
                            noteData = new
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
                            json = JsonSerializer.Serialize(noteData);

                            string newFilename = $"{Path.GetRandomFileName()}.notes.txt";
                            string newFilePath = Path.Combine(FileSystem.AppDataDirectory, newFilename);
                            File.WriteAllText(newFilePath, json);
                            Filename = newFilename; // Update the Filename property to the new file
                            return;
                        }
                    }
                }
            }

            // if directory exists or file exists and is not locked
            // continue to write data...
            File.WriteAllText(filePath, json);
            return;
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
                        noteData = JsonSerializer.Deserialize<Note>(fileContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                        });
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
