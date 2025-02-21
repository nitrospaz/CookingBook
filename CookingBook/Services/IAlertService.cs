namespace CookingBook.Services
{
    internal interface IAlertService
    {
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
        Task DisplayAlert(string title, string message, string cancel);
    }
}
