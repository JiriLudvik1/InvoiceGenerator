using InvoiceGenerator.MAUI.Converters;
using InvoiceGenerator.MAUI.Models;
using System.Text;
using System.Windows.Input;

namespace InvoiceGenerator.MAUI.ViewModels
{
  public class MainPageViewModel : BaseViewModel
  {
    private readonly INavigation navigation;
    public Config Configuration { get; set; }
    public DBQueries DBQueries { get; set; }
    public CustomerModel Customer { get => GetValue<CustomerModel>(); set => SetValue(value); }
    public EmailService EmailService { get; set; }
    public InvoiceDetailModel InvoiceDetail { get => GetValue<InvoiceDetailModel>(); set => SetValue(value); }
    public string UserEmail { get => GetValue<string>(); set => SetValue(value); }
    public string UserPassword { get => GetValue<string>(); set => SetValue(value); }
    public bool UserLoggedIn {get => GetValue<bool>(); set => SetValue(value); }

    public int NextInvoiceNumber { get => GetValue<int>(); set => SetValue(value); }
    public string InvoiceFolder { get => GetValue<string>(); set => SetValue(value); }
    public string InvoiceFileName { get => GetValue<string>(); set => SetValue(value); }

    public MainPageViewModel(INavigation navigation)
    {
      Configuration = Config.InitializeConfigFromDisk();
      this.navigation = navigation;

      DBQueries = new DBQueries(Configuration.ConnectionString);
      string currentYearString = Utils.GetCurrentYearString();
      NextInvoiceNumber = DBQueries.GetNextInvoiceNumber(currentYearString);
      if (NextInvoiceNumber == -1)
      {
        App.AlertService.ShowErrorAsync("Nepodařilo se připojit k databázi!");
      }

      InvoiceDetail = new InvoiceDetailModel();
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

    public ICommand SelectInvoiceFolder => new Command(async () =>
    {
      string folderPath = await Utils.PickFolder();

      if (string.IsNullOrEmpty(folderPath))
      {
        return;
      }

      InvoiceFolder = folderPath;
    });

    #region Invoice generation
    public ICommand GenerateInvoice => new Command(async () =>
    {
      if (Customer is null)
      {
        await App.AlertService.ShowErrorAsync("Nebyl vybrán zákazník");
        return;
      }

      string invoiceFullPath = $"{InvoiceFolder}\\{IntToInvoiceName.StaticConvert(NextInvoiceNumber)}";

      if (InvoiceDetail is null)
      {
        await App.AlertService.ShowErrorAsync("Detail faktury nebyl incializován");
        return;
      }

      var generator = new PDFGenerator(Customer, InvoiceDetail, Configuration, invoiceFullPath);

      if (!generator.GenerateAndSaveInvoicePDF())
      {
        await App.AlertService.ShowErrorAsync("Chyba při generování PDF");
        return;
      }

      await App.AlertService.ShowInfoAsync("PDF bylo vygenerováno");

      if (EmailService is not null && !string.IsNullOrEmpty(Customer.Email))
      {
        await HandleEmailSending(invoiceFullPath, InvoiceDetail);
      }

      string currentYearString = Utils.GetCurrentYearString();
      DBQueries.IncrementInvoiceNumber(currentYearString);
      NextInvoiceNumber = DBQueries.GetNextInvoiceNumber(currentYearString);
    });

    private async Task HandleEmailSending(string invoiceFullPath, InvoiceDetailModel detail)
    {
      bool sendEmail = await App.AlertService.ShowYesNoDialog($"Odeslat fakturu na: {Customer.Email}");
      if (!sendEmail)
      {
        return;
      }

      if (!EmailService.SendInvoice(Customer, detail, invoiceFullPath))
      {
        await App.AlertService.ShowErrorAsync("Nepodařilo se odeslat email");
        return;
      }

      await App.AlertService.ShowInfoAsync("Email byl odeslán!");
    }
    #endregion

    #region Email stuff
    public ICommand LoginButtonClicked => new Command(async () =>
    {
      if (!UserLoggedIn)
      {
        await LoginUser();
        return;
      }

      LogoutUser();
    });

    private async Task LoginUser()
    {
      if (string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword))
      {
        await App.AlertService.ShowErrorAsync("Nevyplněné pole!");
        return;
      }

      var mailAddress = Utils.CreateMailAddress(UserEmail);
      if (mailAddress is null)
      {
        await App.AlertService.ShowErrorAsync("Neplatný email!");
        LogoutUser();
        return;
      }

      EmailService = new EmailService(mailAddress, UserPassword, Configuration.EmailHost, Configuration.EmailPort);
      UserLoggedIn = true;
    }

    private void LogoutUser()
    {
      EmailService = null;
      UserEmail = String.Empty;
      UserPassword = String.Empty;
      UserLoggedIn = false;
    }
    #endregion
  }
}
