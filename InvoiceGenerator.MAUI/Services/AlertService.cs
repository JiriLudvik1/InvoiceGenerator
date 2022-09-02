namespace InvoiceGenerator.MAUI.Services
{
  public interface IAlertService
  {
    Task ShowErrorAsync(string message);
    Task ShowInfoAsync(string message);
    Task<bool> ShowYesNoDialog(string message, string title = "Dotaz");
    Task<string> ShowPrompt(string message, string title, int maxLength = -1);
  }

  internal class AlertService : IAlertService
  {
    public async Task ShowErrorAsync(string message)
    {
      await Application.Current.MainPage.DisplayAlert("Chyba", message, "OK");
    }

    public async Task ShowInfoAsync(string message)
    {
      await Application.Current.MainPage.DisplayAlert("Info", message, "OK");
    }

    public async Task<bool> ShowYesNoDialog(string message, string title = "Dotaz")
    {
      bool answer = await Application.Current.MainPage.DisplayAlert(title, message, "Ano", "Ne");
      return answer;
    }

    public async Task<string> ShowPrompt(string message, string title, int maxLength = -1)
    {
      return await Application.Current.MainPage.DisplayPromptAsync(title, message, "Potvrdit", "Zrušit", string.Empty, maxLength, Keyboard.Default, string.Empty);
    }
  }
}
