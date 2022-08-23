using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace InvoiceGenerator
{
  public class MainPageViewModel : BaseViewModel
  {
    public string ItemsCsvPathText { get => GetValue<string>(); set => SetValue(value); }
    public string ReportPath { get => GetValue<string>(); set => SetValue(value); }
    public Config CurrentConfiguration;

    public MainPageViewModel()
    {
      Task.Run(() => InitializeProgram());
    }

    public void InitializeProgram()
    {
      CurrentConfiguration = GetConfigFile().Result;
      ReportPath = CurrentConfiguration.DefaultReportPath;
    }



    #region Reading Files and contents
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

    private async Task<Config> GetConfigFile()
    {
      StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
      StorageFolder data = await appInstalledFolder.GetFolderAsync("Data");

      string content = await ReadFileContents(await data.GetFileAsync("config.json"));

      return JsonConvert.DeserializeObject<Config>(content);
    }
    #endregion

    private async Task ShowErrorMessage(string message)
    {
      var messageDialog = new MessageDialog(message);
      await messageDialog.ShowAsync();
    }
  }
}
