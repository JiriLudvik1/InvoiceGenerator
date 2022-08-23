using System. Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace InvoiceGenerator
{
  public class BaseViewModel : INotifyPropertyChanged
  {
    public bool IsBusy { get => GetValue<bool>(); set => SetValue(value); }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
      //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

    protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
    {
      if (!properties.ContainsKey(propertyName))
      {
        properties.Add(propertyName, default(T));
      }

      var oldValue = GetValue<T>(propertyName);
      if (!EqualityComparer<T>.Default.Equals(oldValue, value))
      {
        properties[propertyName] = value;
        OnPropertyChanged(propertyName);
      }
    }

    protected T GetValue<T>([CallerMemberName] string propertyName = null)
    {
      if (!properties.ContainsKey(propertyName))
      {
        return default(T);
      }
      else
      {
        return (T)properties[propertyName];
      }
    }

    public virtual Task InitializeAsync(params object[] parameters)
    {
      return Task.FromResult(false);
    }
  }
}
