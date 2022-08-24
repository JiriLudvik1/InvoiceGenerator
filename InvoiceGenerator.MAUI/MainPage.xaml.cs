using InvoiceGenerator.MAUI.Models;
using System.Data;
using System.Text;
using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

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

      ReportPath.Text = Configuration.DefaultReportPath;
      SemanticScreenReader.Announce(ReportPath.Text);

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private async void btPickFile_Clicked(object sender, EventArgs e)
    {
      try
      {
        var selectedFile = await PickFile();

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

    private void TestBut_Clicked(object sender, EventArgs e)
    {

    }

    #region Files and Folders picking
    public async Task<FileResult> PickFile()
    {
      try
      {
        var result = await FilePicker.Default.PickAsync();

        if (result is null)
        {
          return null;
        }

        return result;
      }
      catch
      {
        await DisplayAlert("Chyba!", "Došlo k chybě při načítání souboru", "Zrušit");
        return null;
      }
    }

    public async Task<string> PickFolder()
    {
      var folderPicker = new WindowsFolderPicker();
      folderPicker.FileTypeFilter.Add("*");
      var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

      WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

      var result = await folderPicker.PickSingleFolderAsync();

      return result.Path;
    }
    #endregion

    private void PickCustomer_Clicked(object sender, EventArgs e)
    {
      var customer = DBQueries.GetAllCustomers()[0];
      FillCustomerModule(customer);
    }

    private void FillCustomerModule(Customer customer)
    {
      lblCustomer.Text = customer.Name;
      lblStreet.Text = customer.Street;
      lblCity.Text = customer.City;
      lblZIPCode.Text = customer.ZIPCode;
      lblIC.Text = customer.IC;
      lblDIC.Text = customer.DIC;

      SemanticScreenReader.Announce(lblCustomer.Text);
      SemanticScreenReader.Announce(lblStreet.Text);
      SemanticScreenReader.Announce(lblCity.Text);
      SemanticScreenReader.Announce(lblZIPCode.Text);
      SemanticScreenReader.Announce(lblIC.Text);
      SemanticScreenReader.Announce(lblDIC.Text);
    }
  }
}