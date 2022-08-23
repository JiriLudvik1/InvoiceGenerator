using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
  }
}
