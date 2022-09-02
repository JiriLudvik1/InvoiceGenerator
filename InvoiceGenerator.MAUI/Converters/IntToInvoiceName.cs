using System.Globalization;

namespace InvoiceGenerator.MAUI.Converters
{
  public class IntToInvoiceName : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is int))
      {
        throw new InvalidOperationException("Target must be an integer");
      }

      int newValue = (int)value;
      return $"FAKTURA_{Utils.GetCurrentYearString()}{Utils.GetInvoiceNumberString(newValue)}.pdf";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
