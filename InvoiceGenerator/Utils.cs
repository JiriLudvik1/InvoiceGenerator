using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace InvoiceGenerator
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
  }
}
