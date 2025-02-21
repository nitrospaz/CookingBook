namespace CookingBook.Services
{
    public class AlertService : IAlertService
    {
        private readonly Page _page;

        public AlertService(Page page)
        {
            _page = page;
        }

        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return _page.DisplayAlert(title, message, accept, cancel);
        }

        public Task DisplayAlert(string title, string message, string cancel)
        {
            return _page.DisplayAlert(title, message, cancel);
        }
    }
}
