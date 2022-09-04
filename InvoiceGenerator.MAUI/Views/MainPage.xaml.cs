using InvoiceGenerator.MAUI.Models;
using InvoiceGenerator.MAUI.ViewModels;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace InvoiceGenerator.MAUI
{
  public partial class MainPage : ContentPage
  {
    public string ItemsCSVFilePath { get; set; }
    public DataTable ImportedItems { get; set; }
    public Config Configuration { get; set; }
    public DBQueries DBQueries { get; set; }
    public CustomerModel Customer { get; set; }
    public EmailService EmailService { get; set; }

    public MainPageViewModel ViewModel { get; set; }

    public MainPage()
    {
      InitializeComponent();
      ViewModel = new MainPageViewModel(Navigation);
      BindingContext = ViewModel;
      Configuration = Config.InitializeConfigFromDisk();
      //Tohle není příliš elegantní řešení, možná to oprav
      Task.Run(() => InitializeDb());

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private async Task InitializeDb()
    {
      DBQueries = new DBQueries(Configuration.ConnectionString);
      string newInvoiceName = await GenerateInvoiceName(true);
      if (string.IsNullOrEmpty(newInvoiceName))
      {
        return;
      }
    }

    #region Invoice generation module

    private async void btGenerateInvoice_Clicked(object sender, EventArgs e)
    {
      //if (Customer is null)
      //{
      //  await ShowErrorMessage("Nebyl vybrán zákazník");
      //  return;
      //}

      //string invoiceFullPath = $"{enFolderPath.Text}\\{enFileName.Text}";
      //var detail = await GenerateInvoiceDetail();

      //if (detail is null)
      //{
      //  return;
      //}

      //var generator = new PDFGenerator(Customer, detail, Configuration, invoiceFullPath);

      //if (!generator.GenerateAndSaveInvoicePDF())
      //{
      //  await ShowErrorMessage("Chyba při generování PDF");
      //  return;
      //}

      //await DisplayAlert("Info", "PDF bylo vygenerováno!", "Potvrdit");

      //if (EmailService is not null && !string.IsNullOrEmpty(Customer.Email))
      //{
      //  await HandleEmailSending(invoiceFullPath, detail);
      //}

      //DBQueries.IncrementInvoiceNumber(Utils.GetCurrentYearString());
      //enFileName.Text = await GenerateInvoiceName(true);
    }

    public async Task<string> GenerateInvoiceName(bool returnFileName)
    {
      string yearString = Utils.GetCurrentYearString();
      int invoiceNumber = DBQueries.GetNextInvoiceNumber(yearString);

      if (invoiceNumber == -1)
      {
        await ShowErrorMessage("Nelze získat číslo faktury z db");
        return string.Empty;
      }

      string invoiceNumberString = Utils.GetInvoiceNumberString(invoiceNumber);

      var sb = new StringBuilder();

      if (returnFileName)
      {
        sb.Append("FAKTURA_");
      }
      sb.Append(yearString);

      sb.Append(invoiceNumberString);

      if (returnFileName)
      {
        sb.Append(".pdf");
      }

      return sb.ToString();
    }
    #endregion
    private async Task ShowErrorMessage(string message)
    {
      await DisplayAlert("Chyba", message, "Zrušit");
    }
  }
}