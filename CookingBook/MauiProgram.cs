using CookingBook.Services;
using Microsoft.Extensions.Logging;
using CookingBook.Pages;
using CookingBook.Models;
using Microsoft.EntityFrameworkCore;

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
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "note_record.db");
            builder.Services.AddDbContext<NoteContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

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
    }
}
