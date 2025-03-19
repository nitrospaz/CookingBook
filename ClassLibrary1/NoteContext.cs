using ClassLibrary1.Configuration;
using ClassLibrary1.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1
{
    public class NoteContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }

        // Constructor with no argument is required and it is used when adding/removing migrations from class library
        public NoteContext()
        {
        }

        public NoteContext(DbContextOptions<NoteContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbFilePath = DatabaseConfig.DatabasePath;

            // if you just provide the name of the file, it will be created in the app's local folder
            // if you provide a path to a file, it will be created in that location
            optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define any model relationships here
            // Configure the Note entity
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id); // Set Id as the primary key

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100); // Set Title as required with a max length of 100

                entity.Property(e => e.Rating)
                    .IsRequired(); // Set Rating as required

                entity.Property(e => e.Description)
                    .HasMaxLength(500); // Set Description with a max length of 500

                entity.Property(e => e.Ingredients)
                    .HasMaxLength(1000); // Set Ingredients with a max length of 1000

                entity.Property(e => e.Instructions)
                    .HasMaxLength(2000); // Set Instructions with a max length of 2000

                entity.Property(e => e.Comments)
                    .HasMaxLength(1000); // Set Comments with a max length of 1000

                entity.Property(e => e.Categories)
                    .HasMaxLength(500); // Set Categories with a max length of 500

                entity.Property(e => e.Tags)
                    .HasMaxLength(500); // Set Tags with a max length of 500

                entity.Property(e => e.Author)
                    .HasMaxLength(100); // Set Author with a max length of 100

                entity.Property(e => e.Source)
                    .HasMaxLength(200); // Set Source with a max length of 200

                entity.Property(e => e.SourceUrl)
                    .HasMaxLength(500); // Set SourceUrl with a max length of 500

                entity.Property(e => e.Text)
                    .HasColumnType("TEXT"); // Set Text as a TEXT column

                entity.Property(e => e.DateCreated)
                    .IsRequired(); // Set DateCreated as required

                entity.Property(e => e.DateModified)
                    .IsRequired(); // Set DateModified as required
            });

            base.OnModelCreating(modelBuilder);
        }

        public void EnsureDatabaseCreated()
        {
            // this handles the creation of the database if it doesn't exist
            // it will also apply any pending migrations to bring the database schema up to date
            this.Database.Migrate();
        }

        //private string GetDatabasePath()
        //{
        //    // can't access FileSystem.AppDataDirectory because FileSystem is not available in the class library
        //    // it is only available in the Maui program
        //    //string localFolderPath = Path.Combine(FileSystem.AppDataDirectory, "WCS_CookingBook");

        //    // https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=net-9.0
        //    // LocalApplicationData returns "C:\Users\User\AppData\Local" but actually ends up storing in the
        //    //      C > User > AppData > Local > Packages > {package.name} > LocalCache > Local 
        //    // Environment.SpecialFolder.ApplicationData returns "C:\Users\User\AppData\Roaming" but actually ends up storing in the
        //    //      C > User > AppData > Local > Packages > {package.name} > LocalCache > Roaming
        //    //string localFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WCS_CookingBook");

        //    string localFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WCS_CookingBook");


        //    // This is the current directory where the code is executing from
        //    //string localFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        //    bool dirExist = Directory.Exists(localFolderPath);
        //    if (!Directory.Exists(localFolderPath))
        //    {
        //        Directory.CreateDirectory(localFolderPath);
        //    }

        //    string dbFilePath = Path.Combine(localFolderPath, dbFileName);

        //    return dbFilePath;
        //}
    }
}
