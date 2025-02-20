using CookingBook.Services;
// if it is not implemented for all platforms,
// we need to specify the if for platform-specific implementation
// otherwise it wont compile for other platforms
#if ANDROID
// this is only implemented for android right now
// will probably need to be implemented for other platforms later...
using FileService = CookingBook.Platforms.Android.Services.FileService;
#endif

namespace CookingBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
#if ANDROID
            // Register the FileService with the DependencyService
            DependencyService.Register<IFileService, FileService>();
#endif
            MainPage = new AppShell();
        }
    }
}
