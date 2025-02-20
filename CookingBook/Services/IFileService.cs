namespace CookingBook.Services
{
    public interface IFileService
    {
        Task CreatePdfFileAsync(string fileName);
        Task SavePdfFileAsync(string fileName, byte[] fileContent);
        Task CreateCsvFileAsync(string fileName, string fileContent);

    }
}