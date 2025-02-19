using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics;
using Application = Microsoft.Maui.Controls.Application;
using Button = Microsoft.UI.Xaml.Controls.Button;
using Colors = Microsoft.UI.Colors;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using Thickness = Microsoft.UI.Xaml.Thickness;
using VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CookingBook.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            // set custom initial window size when launched on Windows
            const int width = 800;
            const int height = 600;

            var window = Application.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(width, height));

            // set custom title bar
            var titleBar = appWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;
            titleBar.ButtonBackgroundColor = Colors.Transparent;

            var appTitleBar = new TextBlock
            {
                Text = "Cooking Book",
                FontSize = 32,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };

            var appTitleBarContainer = new Grid
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Children = { appTitleBar }
            };

            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(appTitleBarContainer);
        }
    }
}
