using System.Diagnostics;

namespace CookingBook
{
    public partial class AppShell : Shell
    {
        private bool isNavigating = false;

        public AppShell()
        {
            InitializeComponent();

            // Register routes for the pages
            // Every page that can be navigated to from another page,
            // needs to be registered with the navigation system.
            // The AllNotesPage and AboutPage pages are automatically registered
            // with the navigation system by being declared in the TabBar.

            Routing.RegisterRoute(nameof(Pages.NotePage), typeof(Pages.NotePage));
            //Routing.RegisterRoute(nameof(Pages.AllNotesPage), typeof(Pages.AllNotesPage));
        }

        private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e?.Target?.Location?.OriginalString?.Contains("AllNotesPage") == true && !isNavigating)
            {
                isNavigating = true;
                if (Shell.Current != null && Shell.Current.IsLoaded)
                {
                    try
                    {
                        await Shell.Current.GoToAsync("//AllNotesPage");
                        Debug.WriteLine("Navigating to AllNotesPage");
                    }
                    finally
                    {
                        isNavigating = false;
                    }
                }
                else
                {
                    isNavigating = false;
                }
            }
        }
    }
}
