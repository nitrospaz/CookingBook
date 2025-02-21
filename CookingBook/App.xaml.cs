using CookingBook.Services;
// if it is not implemented for all platforms,
// we need to specify the if for platform-specific implementation
// otherwise it wont compile for other platforms
#if ANDROID
using FileService = CookingBook.Platforms.Android.Services.FileService;
#endif

#if WINDOWS
using FileService = CookingBook.Platforms.Windows.Services.FileService;
#endif


namespace CookingBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // assign main page before registering the services!
            MainPage = new AppShell();

#if ANDROID
            // Register the FileService with the DependencyService
            DependencyService.Register<IFileService, FileService>();
#endif

#if WINDOWS
            // Register the FileService with the DependencyService
            DependencyService.Register<IFileService, FileService>();
#endif

            // Register the alert service
            DependencyService.RegisterSingleton<IAlertService>(new AlertService(MainPage));
        }
    }
}
