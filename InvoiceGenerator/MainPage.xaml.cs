using System;
using System.Data;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InvoiceGenerator
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page, INotifyPropertyChanged 
  {
    private Config _currentConfiguration;
    public Config CurrentConfiguration
    {
      get { return _currentConfiguration; }
      set 
      { 
        _currentConfiguration = value;
        OnPropertyChanged("CurrentConfiguration");
      }
    }

    public MainPage()
    {
      InitializeComponent();
      DataContext = this;
      InitializeProgram();
    }

    public void InitializeProgram()
    {
      //CurrentConfiguration = GetConfigFile().Result;
      CurrentConfiguration = Config.InitializeFromDisk();
    }

    private async void btPickFile_Click(object sender, RoutedEventArgs e)
    {
      StorageFile selectedFile = await PickFile(".csv");
      if (selectedFile is null)
      {
        return;
      }

      string fileContents = await Utils.ReadFileContents(selectedFile);
      if (fileContents.Equals(string.Empty))
      {
        tbCsvPath.Text = String.Empty;
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

    #region Reading Files and contents
    private async Task<bool> ValidateData(DataTable table)
    {
      if (!Utils.TableContainsColumns(Consts.ItemsMandatoryColumns, table, out string missingColumnName))
      {
        await ShowErrorMessage($"Sloupec: {missingColumnName} nebyl nalezen!");
        return false;
      }

      return true;
    }

    private async Task<StorageFile> PickFile(string extension)
    {
      FileOpenPicker filePicker = new FileOpenPicker();
      filePicker.FileTypeFilter.Add(extension);

      StorageFile pickedCsvFile = await filePicker.PickSingleFileAsync();
      return pickedCsvFile;
    }
    #endregion

    private async Task ShowErrorMessage(string message)
    {
      var messageDialog = new MessageDialog(message);
      await messageDialog.ShowAsync();
    }

    #region PropertyChanged stuff
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      GUIThreadDispatcher.Instance.BeginInvoke(() =>
      {
        if (PropertyChanged != null)
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      });
    }
      #endregion
    }
}
