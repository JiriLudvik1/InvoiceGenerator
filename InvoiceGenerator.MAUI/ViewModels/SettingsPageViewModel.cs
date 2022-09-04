using System.Windows.Input;

namespace InvoiceGenerator.MAUI.ViewModels
{
  public class SettingsPageViewModel : BaseViewModel
  {
    public Config Configuration { get => GetValue<Config>(); set => SetValue(value); }
    public bool IsCanceled = true;
    private readonly INavigation navigation;
    private readonly Grid configGrid;

    public SettingsPageViewModel(INavigation navigation, Config config, Grid configGrid)
    {
      this.navigation = navigation;
      Configuration = config;
      this.configGrid = configGrid;
    }

    public ICommand ConfirmChanges => new Command(async () =>
    {
      if (!ValidateInputs())
      {
        await App.AlertService.ShowErrorAsync("Některé pole nebylo správně vyplněno!");
        return;
      }

      IsCanceled = false;

      await navigation.PopAsync();
    });

    private bool ValidateInputs()
    {
      //TODO: finish me
      return true;
    }

    public ICommand SelectDbPath => new Command( async () =>
    {
      var file = await Utils.PickFileAsync();

      if (file is null)
      {
        return;
      }

      //Velmi hacky cesta, jak donutit grid k refreshi
      Configuration.ConnectionString = file.FullPath;
      configGrid.BindingContext = null;
      configGrid.BindingContext = Configuration;
    });

    public ICommand CancelChanges => new Command(() =>
    {
      IsCanceled = true;
      navigation.PopAsync();
    });
  }
}
