using System.Globalization;

namespace InvoiceGenerator.MAUI.Converters
{
  public class BoolToUserLoggedInString : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is bool))
      {
        throw new InvalidOperationException("Target must be a boolean");
      }

      if ((bool)value)
      {
        return "Odhlásit";
      }
      return "Přihlásit";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
