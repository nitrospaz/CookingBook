using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CookingBook.Platforms.Android.Services;
using CookingBook.Services;

namespace CookingBook
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;

            // Request permissions for older compatibility less than Android 10 API 29
            RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            var fileService = DependencyService.Get<IFileService>() as FileService;
            fileService?.OnActivityResult(requestCode, resultCode, data);
        }
    }
}
