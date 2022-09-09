using InvoiceGenerator.MAUI.Models;
using InvoiceGenerator.MAUI.ViewModels;

namespace InvoiceGenerator.MAUI
{
  public partial class PickFromList : ContentPage
  {
    public PickCustomerViewModel ViewModel { get; set; }

    public PickFromList(List<CustomerModel> customerList)
    {
      InitializeComponent();
      ViewModel = new PickCustomerViewModel(Navigation, customerList);
      BindingContext = ViewModel;
    }
  }
}