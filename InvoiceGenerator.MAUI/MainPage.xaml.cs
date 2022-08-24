using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using System.Diagnostics;
using System.Text;
using Windows.Storage.Pickers;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

namespace InvoiceGenerator.MAUI
{
  public partial class MainPage : ContentPage
  {
    public string ItemsCSVFilePath { get; set; }
    public DataTable ImportedItems { get; set; }
    public Config Configuration { get; set; }
    public DBQueries DBQueries { get; set; }

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
      // Make it work for Windows 10
      folderPicker.FileTypeFilter.Add("*");
      // Get the current window's HWND by passing in the Window object
      var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

      // Associate the HWND with the file picker
      WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

      var result = await folderPicker.PickSingleFolderAsync();

      return result.Path;
    }

    private async void TestBut_Clicked(object sender, EventArgs e)
    {
      try
      {
        // Create a new PDF document
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Created with PDFsharp";

        // Create an empty page
        PdfPage page = document.AddPage();

        // Get an XGraphics object for drawing
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Create a font
        XFont font = new XFont("Arial", 15);

        // Draw the text
        gfx.DrawString("Hello, World!", font, XBrushes.Black,
          new XRect(0, 0, page.Width, page.Height),
          XStringFormats.Center);

        // Save the document...
        string folderPath = await PickFolder();
        string filePath = $"{folderPath}\\test.pdf";

        document.Save(filePath);
        // ...and start a viewer.
        Process.Start(filePath);
      }
      catch (Exception ex)
      {

      }

    }
  }
}