using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace CookingBook.ViewModels
{
    internal class AboutViewModel
    {
        public string Title => AppInfo.Name;
        public string Version => AppInfo.VersionString;
        public string MoreInfoUrl => "https://www.walkercrystalsoftware.com";
        public string Message => "This app is written by Wlaker Crystal Software.";
        // Command is equivalent to onClicked in Xamarin.Forms
        public ICommand ShowMoreInfoCommand { get; }

        public AboutViewModel()
        {
            ShowMoreInfoCommand = new AsyncRelayCommand(ShowMoreInfo);
        }

        async Task ShowMoreInfo() =>
            await Launcher.Default.OpenAsync(MoreInfoUrl);
    }
}
