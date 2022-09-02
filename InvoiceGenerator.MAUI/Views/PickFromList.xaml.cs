using InvoiceGenerator.MAUI.Models;

namespace InvoiceGenerator.MAUI
{
  public partial class PickFromList : ContentPage
  {
    public readonly List<CustomerModel> CustomerList;
    public CustomerModel SelectedCustomer { get; set; }
    public bool IsCanceled { get; set; } = true;

    public PickFromList(List<CustomerModel> customerList)
    {
      InitializeComponent();
      CustomerList = customerList;
      customers.ItemsSource = CustomerList;

      btConfirm.IsEnabled = false;
    }

    private async void btConfirm_Clicked(object sender, EventArgs e)
    {
      if (customers.SelectedItem is null)
      {
        await DisplayAlert("Chyba", "Nebyl vybrán žádný zákazník", "Zpìt");
        return;
      }

      SelectedCustomer = customers.SelectedItem as CustomerModel;
      IsCanceled = false;
      await Navigation.PopAsync();
    }

    private async void btCancel_Clicked(object sender, EventArgs e)
    {
      IsCanceled = true;
      await Navigation.PopAsync();
    }

    private void customers_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
      btConfirm.IsEnabled = true;
    }
  }
}