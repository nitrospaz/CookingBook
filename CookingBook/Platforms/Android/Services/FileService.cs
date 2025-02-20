using Android.App;
using Android.Content;
using Android.Provider;
using CookingBook.Platforms.Android.Services;
using CookingBook.Services;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(CookingBook.Platforms.Android.Services.FileService))]
namespace CookingBook.Platforms.Android.Services
{
    public class FileService : IFileService
    {
        private const int CreateFileRequestCode = 1;
        private TaskCompletionSource<bool> _taskCompletionSource;

        public async Task CreateCsvFileAsync(string fileName, string fileContent)
        {
            var contentResolver = Application.Context.ContentResolver;
            var values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "text/csv");
            values.Put(MediaStore.IMediaColumns.RelativePath, Environment.DirectoryDownloads);

            var uri = contentResolver.Insert(MediaStore.Downloads.ExternalContentUri, values);

            if (uri != null)
            {
                using (var outputStream = contentResolver.OpenOutputStream(uri))
                {
                    using (var writer = new StreamWriter(outputStream))
                    {
                        await writer.WriteAsync(fileContent);
                    }
                }
            }
        }

        public Task CreatePdfFileAsync(string fileName)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            var intent = new Intent(Intent.ActionCreateDocument);
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("application/pdf");
            intent.PutExtra(Intent.ExtraTitle, fileName);

            var activity = MainActivity.Instance;
            activity.StartActivityForResult(intent, CreateFileRequestCode);

            return _taskCompletionSource.Task;
        }

        public async Task SavePdfFileAsync(string fileName, byte[] fileContent)
        {
            var contentResolver = Application.Context.ContentResolver;
            var values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "application/pdf");
            values.Put(MediaStore.IMediaColumns.RelativePath, Environment.DirectoryDownloads);

            var uri = contentResolver.Insert(MediaStore.Downloads.ExternalContentUri, values);

            if (uri != null)
            {
                using (var outputStream = contentResolver.OpenOutputStream(uri))
                {
                    await outputStream.WriteAsync(fileContent, 0, fileContent.Length);
                }
            }
        }

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == CreateFileRequestCode)
            {
                _taskCompletionSource.SetResult(resultCode == Result.Ok);
            }
        }
    }
}
