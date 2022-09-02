using InvoiceGenerator.MAUI.Models;
using System.Data;
using System.Text;
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
    public int NextInvoiceNumber { get => GetValue<int>(); set => SetValue(value); }

    public MainPageViewModel(INavigation navigation)
    {
      Configuration = Config.InitializeConfigFromDisk();
      InitializeDb();
      this.navigation = navigation;
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private void InitializeDb()
    {
      DBQueries = new DBQueries(Configuration.ConnectionString);
      string currentYearString = Utils.GetCurrentYearString();
      NextInvoiceNumber = DBQueries.GetNextInvoiceNumber(currentYearString);
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

    public ICommand GenerateInvoice => new Command(() =>
    {
      NextInvoiceNumber++;
    });
  }
}
