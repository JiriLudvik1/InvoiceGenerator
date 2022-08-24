using InvoiceGenerator.MAUI.Models;

namespace InvoiceGenerator.MAUI
{
  public partial class PickFromList : ContentPage
  {
    public readonly List<Customer> CustomerList;
    public Customer SelectedCustomer { get; set; }
    public bool IsCanceled { get; set; } = false;

    public PickFromList(List<Customer> customerList)
    {
      InitializeComponent();
      CustomerList = customerList;
      customers.ItemsSource = CustomerList;
    }

    private async void btConfirm_Clicked(object sender, EventArgs e)
    {
      if (customers.SelectedItem is null)
      {
        await DisplayAlert("Chyba", "Nebyl vybrán žádný zákazník", "Zpìt");
        return;
      }

      SelectedCustomer = customers.SelectedItem as Customer;
      await Navigation.PopAsync();
    }

    private async void btCancel_Clicked(object sender, EventArgs e)
    {
      IsCanceled = true;
      await Navigation.PopAsync();
    }
  }
}