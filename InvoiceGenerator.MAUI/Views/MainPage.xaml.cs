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
    public Customer Customer { get; set; }
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
      enFileName.Text = newInvoiceName;
    }

    private async void btPickFile_Clicked(object sender, EventArgs e)
    {
      //try
      //{
      //  var selectedFile = await Utils.PickFile();

      //  if (selectedFile is null)
      //  {
      //    return;
      //  }

      //  string fileContents = Utils.ReadFileContents(selectedFile.FullPath);
      //  ImportedItems = null;
      //  ImportedItems = Utils.CSVStringToDataTable(fileContents, Consts.csvDelimiter);

      //  if (ImportedItems is null)
      //  {
      //    ItemsCSVFilePath = String.Empty;
      //    return;
      //  }

      //  if (Utils.TableContainsColumns(Consts.ItemsMandatoryColumns, ImportedItems, out string missingColumn))
      //  {
      //    await ShowErrorMessage($"Chybí sloupec {missingColumn}");
      //    ImportedItems = null;
      //    ItemsCSVFilePath = String.Empty;
      //    return;
      //  }

      //  ItemsCSVFilePath = selectedFile.FullPath;
      //}
      //finally
      //{
      //  tbCsvPath.Text = ItemsCSVFilePath;
      //  SemanticScreenReader.Announce(tbCsvPath.Text);
      //}
    }

    #region Invoice generation module
    private async void btPickFolder_Clicked(object sender, EventArgs e)
    {
      string folderPath = await Utils.PickFolder();

      if (string.IsNullOrEmpty(folderPath))
      {
        return;
      }

      enFolderPath.Text = folderPath;
    }

    private async void btGenerateInvoice_Clicked(object sender, EventArgs e)
    {
      if (Customer is null)
      {
        await ShowErrorMessage("Nebyl vybrán zákazník");
        return;
      }

      string invoiceFullPath = $"{enFolderPath.Text}\\{enFileName.Text}";
      var detail = await GenerateInvoiceDetail();

      if (detail is null)
      {
        return;
      }

      var generator = new PDFGenerator(Customer, detail, Configuration, invoiceFullPath);

      if (!generator.GenerateAndSaveInvoicePDF())
      {
        await ShowErrorMessage("Chyba při generování PDF");
        return;
      }

      await DisplayAlert("Info", "PDF bylo vygenerováno!", "Potvrdit");

      if (EmailService is not null && !string.IsNullOrEmpty(Customer.Email))
      {
        await HandleEmailSending(invoiceFullPath, detail);
      }

      DBQueries.IncrementInvoiceNumber(Utils.GetCurrentYearString());
      enFileName.Text = await GenerateInvoiceName(true);
    }

    private async Task HandleEmailSending(string invoiceFullPath, InvoiceDetail detail)
    {
      bool sendEmail = await DisplayAlert("Odeslat emailem?", $"Odeslat fakturu na: {Customer.Email}?", "Ano", "Ne");
      if (!sendEmail)
      {
        return;
      }

      if(!EmailService.SendInvoice(Customer, detail, invoiceFullPath))
      {
        await ShowErrorMessage("Nepodařilo se odeslat email!");
        return;
      }

      await DisplayAlert("Info", "Email byl odeslán!", "Zrušit");
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

    private async Task<InvoiceDetail> GenerateInvoiceDetail()
    {
      InvoiceDetail invoiceDetail = new InvoiceDetail();

      invoiceDetail.Number = await GenerateInvoiceName(false);
      invoiceDetail.InvoiceName = await GenerateInvoiceName(true);
      invoiceDetail.PaymentDue = DateOnly.FromDateTime(dpPaymentDue.Date);
      invoiceDetail.CreatedDate = DateOnly.FromDateTime(dpCreatedDate.Date);
      invoiceDetail.DateOfTaxableSupply = DateOnly.FromDateTime(dpDateOfTaxableSupply.Date);
      invoiceDetail.InstalationPreset = true;

      if(!Double.TryParse(enPresetPrice.Text, out double presetPrice))
      {
        await ShowErrorMessage("Neplatná cena");
        return null;
      }

      invoiceDetail.PresetInstalationPrice = presetPrice;
      invoiceDetail.Attachments = enAttachments.Text;

      return invoiceDetail;
    }
    #endregion

    #region Top and bottom menu handlers
    private async void btLoginEmail_Clicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(enEmail.Text) || string.IsNullOrEmpty(enPassword.Text))
      {
        await ShowErrorMessage("Nevyplněné pole!");
        return;
      }

      var mailAddress = CreateMailAddress(enEmail.Text);
      if (mailAddress is null)
      {
        await ShowErrorMessage("Neplatná emailová adresa!");
        enEmail.Text = String.Empty;
        enPassword.Text = String.Empty;
        return;
      }

      EmailService = new EmailService(mailAddress, enPassword.Text, Configuration.EmailHost, Configuration.EmailPort);

      enPassword.IsEnabled = false;
      enEmail.IsEnabled = false;
      btLoginEmail.Text = "Přihlášen!";
      btLoginEmail.IsEnabled = false;
    }

    private async void btSettings_Clicked(object sender, EventArgs e)
    {
      var settingsPage = new SettingsPage(Configuration);

      await Utils.ShowPageAsDialog(Navigation, settingsPage);

      if (settingsPage.IsCanceled)
      {
        return;
      }

      Configuration = settingsPage.Configuration;
      Config.SaveConfigToDisk(Configuration);
      DBQueries = new DBQueries(Configuration.ConnectionString);
    }
    #endregion

    private MailAddress CreateMailAddress(string email)
    {
      try
      {
        return new MailAddress(email);
      }
      catch 
      {
        return null;
      }
    }

    private async Task ShowErrorMessage(string message)
    {
      await DisplayAlert("Chyba", message, "Zrušit");
    }
  }
}