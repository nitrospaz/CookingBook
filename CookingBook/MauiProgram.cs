using ClassLibrary1.Configuration;
using CookingBook.Pages;
using CookingBook.Services;
using Microsoft.Extensions.Logging;

namespace CookingBook
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register the NoteContext with dependency injection
            // this does not actually work. The database is not created.
            // I have to create the db from the NoteContext OnConfiguring method
            //string dbPath = GetDatabasePath();
            //builder.Services.AddDbContext<NoteContext>(
            //    options => options.UseSqlite($"Data Source={dbPath}", x => x.MigrationsAssembly(nameof(ClassLibrary1))));

            // Get the database path
            string dbPath = GetDatabasePath();
            // set variable that can be read from library
            DatabaseConfig.DatabasePath = dbPath;

            // Register the ISqLiteService
            builder.Services.AddSingleton<ISqLiteService, SqLiteService>();

            // Register the AllNotesPage
            builder.Services.AddTransient<AllNotesPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<ExportRecipesPage>();
            builder.Services.AddTransient<NotePage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        public static string GetDatabasePath()
        {
            var databasePath = "";
            var databaseName = "note_record.db3";

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                SQLitePCL.Batteries_V2.Init();
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // this creates the final path to the database file
                // "C:\\Users\\User\\AppData\\Local\\Packages\\{App.Name}\\LocalState\\note_record.db3"
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }
            else
            {
                databasePath = databaseName;
            }

            return databasePath;

        }
    }
}
