using InvoiceGenerator.MAUI.Services;

namespace InvoiceGenerator.MAUI
{
  public partial class App : Application
  {
    public static IServiceProvider Services;
    public static IAlertService AlertService;
    public App(IServiceProvider provider)
    {
      InitializeComponent();
      Current.UserAppTheme = AppTheme.Light;

      Services = provider;
      AlertService = Services.GetService<IAlertService>();
 
      MainPage = new AppShell();
    }
  }
}