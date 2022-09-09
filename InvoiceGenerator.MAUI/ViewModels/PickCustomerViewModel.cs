using InvoiceGenerator.MAUI.Models;
using System.Windows.Input;

namespace InvoiceGenerator.MAUI.ViewModels
{
  public class PickCustomerViewModel : BaseViewModel
  {
    private readonly INavigation navigation;
    private readonly List<CustomerModel> defaultCustomers;
    public List<CustomerModel> VisibleCustomers { get => GetValue<List<CustomerModel>>(); set => SetValue(value); }
    public CustomerModel SelectedCustomer { get => GetValue<CustomerModel>(); set => SetValue(value); }
    public bool IsCanceled = true;
    public string NameFilter { get => GetValue<string>(); set => SetValue(value); }

    public PickCustomerViewModel(INavigation navigation, List<CustomerModel> defaultCustomers)
    {
      this.navigation = navigation;
      this.defaultCustomers = defaultCustomers;

      VisibleCustomers = defaultCustomers;
    }

    public ICommand Confirm => new Command(async () =>
    {
      if (SelectedCustomer is null)
      {
        await App.AlertService.ShowErrorAsync("Nebyl vybrán žádný zákazník");
        return;
      }

      IsCanceled = false;
      await navigation.PopAsync();
    });

    public ICommand FilterByName => new Command(() =>
    {
      SelectedCustomer = null;
      SelectedCustomer = null;

      if (string.IsNullOrEmpty(NameFilter))
      {
        VisibleCustomers = defaultCustomers;
        return;
      }

      VisibleCustomers = defaultCustomers.FindAll(x => x.Name.Contains(NameFilter));
    });

    public ICommand Cancel => new Command(async () =>
    {
      IsCanceled = true;
      await navigation.PopAsync();
    });
  }
}
