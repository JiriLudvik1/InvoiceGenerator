using InvoiceGenerator.MAUI.Models;
using Microsoft.Maui.Controls;
using System.Data;
using System.Windows.Input;

namespace InvoiceGenerator.MAUI.ViewModels
{
  public class MainPageViewModel : BaseViewModel
  {
    private readonly INavigation navigation;

    public string ItemsCSVFilePath { get; set; }
    public DataTable ImportedItems { get; set; }
    public Config Configuration { get => GetValue<Config>(); set => SetValue(value); }
    public DBQueries DBQueries { get => GetValue<DBQueries>(); set => SetValue(value); }
    public Customer Customer { get => GetValue<Customer>(); set => SetValue(value); }
    public EmailService EmailService { get; set; }


    public MainPageViewModel(INavigation navigation)
    {
      Configuration = Config.InitializeConfigFromDisk();
      InitializeDb();
      this.navigation = navigation;
    }

    private void InitializeDb()
    {
      DBQueries = new DBQueries(Configuration.ConnectionString);
      //string newInvoiceName = await GenerateInvoiceName(true);
      //if (string.IsNullOrEmpty(newInvoiceName))
      //{
      //  return;
      //}
      //enFileName.Text = newInvoiceName;
    }

    public ICommand PickCustomer => new Command(async () =>
    {
      var customers = DBQueries.GetAllCustomers();
      var customerSelectPage = new PickFromList(customers);

      await Utils.ShowPageAsDialog(navigation, customerSelectPage);

      if (customerSelectPage.IsCanceled)
      {
        return;
      }

      Customer = customerSelectPage.SelectedCustomer;
    });
  }
}
