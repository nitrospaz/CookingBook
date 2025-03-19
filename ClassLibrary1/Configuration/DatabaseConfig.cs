namespace ClassLibrary1.Configuration
{
    public static class DatabaseConfig
    {
        // can't access FileSystem.AppDataDirectory because FileSystem is not available in the class library
        // it is only available in the Maui program
        // so i made a global variable that I set from the maui app and read from the library
        // i default it to just the name of the database file in case it is not set
        public static string DatabasePath { get; set; } = "note_record.db3";
    }
}
