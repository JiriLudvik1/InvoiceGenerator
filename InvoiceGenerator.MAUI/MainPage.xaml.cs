﻿using InvoiceGenerator.MAUI.Models;
using System.Data;
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

    public MainPage()
    {
      InitializeComponent();
      Configuration = Config.InitializeConfigFromDisk();
      DBQueries = new DBQueries(Configuration.ConnectionString);

      enFileName.Text = GenerateInvoiceName();

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private async void btPickFile_Clicked(object sender, EventArgs e)
    {
      try
      {
        var selectedFile = await Utils.PickFile();

        if (selectedFile is null)
        {
          return;
        }

        string fileContents = Utils.ReadFileContents(selectedFile.FullPath);
        ImportedItems = null;
        ImportedItems = Utils.CSVStringToDataTable(fileContents, Consts.csvDelimiter);

        if (ImportedItems is null)
        {
          ItemsCSVFilePath = String.Empty;
          return;
        }

        if (Utils.TableContainsColumns(Consts.ItemsMandatoryColumns, ImportedItems, out string missingColumn))
        {
          await DisplayAlert("Chyba", $"Chybí sloupec {missingColumn}", "Zrušit");
          ImportedItems = null;
          ItemsCSVFilePath = String.Empty;
          return;
        }

        ItemsCSVFilePath = selectedFile.FullPath;
      }
      finally
      {
        tbCsvPath.Text = ItemsCSVFilePath;
        SemanticScreenReader.Announce(tbCsvPath.Text);
      }
    }

    #region Customer module
    private async void PickCustomer_Clicked(object sender, EventArgs e)
    {
      var customers = DBQueries.GetAllCustomers();
      var customerSelectPage = new PickFromList(customers);

      await Utils.ShowPageAsDialog(Navigation ,customerSelectPage);

      if (customerSelectPage.IsCanceled)
      {
        return;
      }

      FillCustomerModule(customerSelectPage.SelectedCustomer);
      Customer = customerSelectPage.SelectedCustomer;
    }

    private void FillCustomerModule(Customer customer)
    {
      lblCustomer.Text = customer.Name;
      lblStreet.Text = customer.Street;
      lblCity.Text = customer.City;
      lblZIPCode.Text = customer.ZIPCode;
      lblIC.Text = customer.IC;
      lblDIC.Text = customer.DIC;
    }
    #endregion

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

    private void btGenerateInvoice_Clicked(object sender, EventArgs e)
    {
      if (Customer is null)
      {
        DisplayAlert("Chyba", "Nebyl vybrán zákazník", "Zrušit");
        return;
      }

      string invoiceFullPath = $"{enFolderPath.Text}\\{enFileName.Text}";
      var detail = GenerateInvoiceDetail();

      if (detail is null)
      {
        return;
      }

      var generator = new PDFGenerator(Customer, detail, Configuration, invoiceFullPath);

      if (!generator.GenerateAndSaveInvoicePDF())
      {
        DisplayAlert("Chyba", "Chyba při generování PDF", "Zrušit");
        return;
      }

      DisplayAlert("Info", "PDF bylo vygenerováno!", "Potvrdit");


      DBQueries.IncrementInvoiceNumber("2022");
      enFileName.Text = GenerateInvoiceName();
    }

    public string GenerateInvoiceName()
    {
      string yearString = Utils.GetCurrentYearString();
      int invoiceNumber = DBQueries.GetNextInvoiceNumber(yearString);

      if (invoiceNumber == -1)
      {
        DisplayAlert("Chyba", "Nelze získat číslo faktury z db", "Zrušit");
        return string.Empty;
      }

      string invoiceNumberString = Utils.GetInvoiceNumberString(invoiceNumber);

      var sb = new StringBuilder();
      sb.Append("FAKTURA_");
      sb.Append(yearString);
      sb.Append(invoiceNumberString);
      sb.Append(".pdf");

      return sb.ToString();
    }

    private InvoiceDetail GenerateInvoiceDetail()
    {
      InvoiceDetail invoiceDetail = new InvoiceDetail();

      invoiceDetail.InvoiceName = GenerateInvoiceName();
      invoiceDetail.PaymentDue = DateOnly.FromDateTime(dpPaymentDue.Date);
      invoiceDetail.CreatedDate = DateOnly.FromDateTime(dpCreatedDate.Date);
      invoiceDetail.DateOfTaxableSupply = DateOnly.FromDateTime(dpDateOfTaxableSupply.Date);
      invoiceDetail.InstalationPreset = true;

      if(!Double.TryParse(enPresetPrice.Text, out double presetPrice))
      {
        DisplayAlert("Chyba", "Neplatná cena!", "Zrušit");
        return null;
      }

      invoiceDetail.PresetInstalationPrice = presetPrice;
      invoiceDetail.Attachments = enAttachments.Text;

      return invoiceDetail;
    }
    #endregion

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
  }
}