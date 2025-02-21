using CookingBook.Services;
using CookingBook.Utilities;
using System.Diagnostics;
using System.Text;

[assembly: Dependency(typeof(CookingBook.Platforms.Windows.Services.FileService))]
namespace CookingBook.Platforms.Windows.Services
{
    public class FileService : IFileService
    {
        private readonly IAlertService _alertService;

        public FileService()
        {
            _alertService = DependencyService.Get<IAlertService>();
        }

        public async Task CreateCsvFileAsync(string fileName, string fileContent)
        {
            // Get the file path
            string filePath = string.Empty;

            // For Windows, save the file in the Documents folder
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            filePath = Path.Combine(documentsPath, fileName);

            // check to see if the file exists
            if (File.Exists(filePath))
            {
                // If the file exists, ask the user if they want to overwrite it
                var overwrite = await _alertService.DisplayAlert("File Exists", $"The file {fileName} already exists. Do you want to overwrite it?", "Yes", "No");
                if (!overwrite)
                {
                    // If the user does not want to overwrite the file, return
                    return;
                }
                else
                {
                    // if they want to overwrite, check to see if the file is locked
                    if (FileAccessUtility.IsFileLocked(filePath))
                    {
                        await _alertService.DisplayAlert("File In Use.", $"The file {fileName} is currently in use. Please close the file and try again.", "OK");
                        return;
                    }
                }
            }
            else
            {
                // If the file does not exist, check to see if the directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    try
                    {
                        // If the directory does not exist, create it
                        Directory.CreateDirectory(directory);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Directory not created. Error: " + ex);
                        await _alertService.DisplayAlert("Export Failed", $"Something went wrong. Directory not available.", "OK");
                        return;
                    }
                }
            }

            try
            {
                // Save the CSV file with UTF-8 encoding
                File.WriteAllText(filePath, fileContent, Encoding.UTF8);

                // Notify the user
                await _alertService.DisplayAlert("Export Successful", $"Recipes exported to {filePath}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Write not successful. Error: " + ex);

                // Notify the user
                await _alertService.DisplayAlert("Export Failed", $"Error trying to write the file.", "OK");
            }
        }

        public async Task CreatePdfFileAsync(string fileName)
        {
            //var savePicker = new FileSavePicker
            //{
            //    SuggestedStartLocation = PickerLocationId.Downloads
            //};
            //savePicker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });
            //savePicker.SuggestedFileName = fileName;

            //StorageFile file = await savePicker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    // Create an empty PDF file
            //    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        // You can add PDF content creation logic here
            //    }
            //}
            return;
        }

        public async Task SavePdfFileAsync(string fileName, byte[] fileContent)
        {
            //var savePicker = new FileSavePicker
            //{
            //    SuggestedStartLocation = PickerLocationId.Downloads
            //};
            //savePicker.FileTypeChoices.Add("PDF", new List<string> { ".pdf" });
            //savePicker.SuggestedFileName = fileName;

            //StorageFile file = await savePicker.PickSaveFileAsync();
            //if (file != null)
            //{
            //    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            //    {
            //        using (var outputStream = stream.AsStreamForWrite())
            //        {
            //            await outputStream.WriteAsync(fileContent, 0, fileContent.Length);
            //        }
            //    }
            //}
            return;
        }
    }
}