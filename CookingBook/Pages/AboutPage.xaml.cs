namespace CookingBook.Pages;

public partial class AboutPage : ContentPage
{
    public string MoreInfoUrl => "https://www.walkercrystalsoftware.com";

    public AboutPage()
	{
		InitializeComponent();
        InitializeUI();
    }

    void InitializeUI()
    {
        TitleLabel.Text = AppInfo.Name;
        VersionLabel.Text = AppInfo.VersionString;
        MessageLabel.Text = "Welcome to CookingBook! \n \n This app was written by Walker Crystal Software. \n";

    }

    private async void OnLearnMoreClicked(object sender, EventArgs e)
    {
        await Launcher.OpenAsync(new Uri(MoreInfoUrl));
    }

}