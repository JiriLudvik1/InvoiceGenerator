using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InvoiceGenerator
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    private async void btPickFile_Click(object sender, RoutedEventArgs e)
    {
      StorageFile selectedFile = await PickFile(".csv");
      if (selectedFile is null)
      {
        return;
      }

      string fileContents = await ReadFileContents(selectedFile);
      if (fileContents.Equals(string.Empty))
      {
        return;
      }

      DataTable table = Utils.CSVStringToDataTable(fileContents, Consts.csvDelimiter);

      if (table is null)
      {
        await ShowErrorMessage("Došlo k neočekávané vyjimce");
        return;
      }

      if (!ValidateData(table).Result)
      {
        return;
      }

      tbCsvPath.Text = selectedFile.Path;
    }

    private async Task<bool> ValidateData(DataTable table)
    {
      if (!table.Columns.Contains(Consts.column_ItemName))
      {
        await ShowErrorMessage($"Sloupec: {Consts.column_ItemName} nebyl nalezen!");
        return false;
      }

      if (!table.Columns.Contains(Consts.column_ItemCount))
      {
        await ShowErrorMessage($"Sloupec: {Consts.column_ItemCount} nebyl nalezen!");
        return false;
      }

      if (!table.Columns.Contains(Consts.column_ItemPrice))
      {
        await ShowErrorMessage($"Sloupec: {Consts.column_ItemPrice} nebyl nalezen!");
        return false;
      }

      return true;
    }

    private async Task<string> ReadFileContents(StorageFile selectedFile)
    {
      if (selectedFile is null)
      {
        return string.Empty;
      }
      return await FileIO.ReadTextAsync(selectedFile); ;
    }

    private async Task<StorageFile> PickFile(string extension)
    {
      FileOpenPicker filePicker = new FileOpenPicker();
      filePicker.FileTypeFilter.Add(extension);

      StorageFile pickedCsvFile = await filePicker.PickSingleFileAsync();
      return pickedCsvFile;
    }

    private async Task ShowErrorMessage(string message)
    {
      var messageDialog = new MessageDialog(message);
      await messageDialog.ShowAsync();
    }
  }
}
