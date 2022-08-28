using System.Data;
using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;


namespace InvoiceGenerator.MAUI
{
  public static class Utils
  {
    public static DataTable CSVStringToDataTable(string fileContents, char csvDelimiter)
    {
      try
      {
        DataTable dt = new DataTable();

        string[] tableData = fileContents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        var col = from cl in tableData[0].Split(csvDelimiter)
                  select new DataColumn(cl);
        dt.Columns.AddRange(col.ToArray());

        (from st in tableData.Skip(1)
         select dt.Rows.Add(st.Split(csvDelimiter))).ToList();
        return dt;
      }
      catch
      {
        return null;
      }
    }

    public static bool TableContainsColumns(List<string> columnList, DataTable table, out string missingColumn)
    {
      missingColumn = string.Empty;
      foreach (var columnName in columnList)
      {
        if (!table.Columns.Contains(columnName))
        {
          missingColumn = columnName;
          return false;
        }
      }
      return true;
    }

    public static string ReadFileContents(string path)
    {
      try
      {
        return File.ReadAllText(path);
      }
      catch 
      {
        return string.Empty;
      }
    }

    public static async Task ShowPageAsDialog(this INavigation navigation, Page page)
    {
      int pagesOnStack = navigation.NavigationStack.Count + 1;
      var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      page.Disappearing += (s, e) =>
      {
        if (navigation.NavigationStack.Count <= pagesOnStack)
          waitHandle.Set();
      };
      await navigation.PushAsync(page);
      await Task.Run(() => waitHandle.WaitOne());
    }

    #region Files and Folders picking
    public static async Task<FileResult> PickFile()
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
        return null;
      }
    }

    public static async Task<string> PickFolder()
    {
      var folderPicker = new WindowsFolderPicker();
      folderPicker.FileTypeFilter.Add("*");
      var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

      WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

      var result = await folderPicker.PickSingleFolderAsync();
      if (result is null)
      {
        return string.Empty;
      }

      return result.Path;
    }
    #endregion


    public static string GetCurrentYearString()
    {
      return DateTime.Now.Year.ToString();
    }

    public static string GetInvoiceNumberString(int number)
    {
      return number.ToString().PadLeft(3,'0');
    }

  } 
}
