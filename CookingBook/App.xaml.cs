using CookingBook.Services;
using FileService = CookingBook.Platforms.Android.Services.FileService;

namespace CookingBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register the FileService with the DependencyService
            DependencyService.Register<IFileService, FileService>();

            MainPage = new AppShell();
        }
    }
}
