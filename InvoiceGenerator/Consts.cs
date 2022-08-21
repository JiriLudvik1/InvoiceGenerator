using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator
{
  public static class Consts
  {
    public static char csvDelimiter { get; } = ';';
    public static string column_ItemCount { get; } = "pocet";
    public static string column_ItemName { get; } = "polozka";
    public static string column_ItemPrice { get; } = "cena";

  }
}
