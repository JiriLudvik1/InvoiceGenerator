using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace InvoiceGenerator
{
  public static class Consts
  {
    public static char csvDelimiter { get; } = ';';
    public static string column_ItemCount { get; } = "pocet";
    public static string column_ItemName { get; } = "polozka";
    public static string column_ItemPrice { get; } = "cena";
    public static List<string> ItemsMandatoryColumns = new List<string>() { column_ItemCount, column_ItemName, column_ItemPrice };
  }
}
