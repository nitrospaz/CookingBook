namespace CookingBook
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for the pages
            // Every page that can be navigated to from another page,
            // needs to be registered with the navigation system.
            // The AllNotesPage and AboutPage pages are automatically registered
            // with the navigation system by being declared in the TabBar.
            Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));
        }
    }
}
