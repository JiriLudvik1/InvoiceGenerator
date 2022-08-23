using System.Data;

namespace InvoiceGenerator.MAUI
{
  public partial class MainPage : ContentPage
  {
    public string ItemsCSVFilePath { get; set; }
    public DataTable ImportedItems { get; set; }
    public Config Configuration { get; set; }

    public MainPage()
    {
      InitializeComponent();
      Configuration = Config.InitializeConfigFromDisk();

      ReportPath.Text = Configuration.DefaultReportPath;
      SemanticScreenReader.Announce(ReportPath.Text);
    }

    private async void btPickFile_Clicked(object sender, EventArgs e)
    {
      try
      {
        var selectedFile = await PickFile();

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
  }
}