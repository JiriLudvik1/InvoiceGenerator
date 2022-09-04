using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Windows.Input;

namespace InvoiceGenerator.MAUI
{
  public class Config
  {
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string ZLNumber { get; set; }
    public string IC { get; set; }
    public string DIC { get; set; }
    public string AccountNumber { get; set; }
    public string BankName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string ZIPCode { get; set; }
    public string ConstantSymbol { get; set; }
    public string DefaultReportPath { get; set; }
    public string ConnectionString { get; set; }
    public string EmailHost { get; set; }
    public int EmailPort { get; set; }

    private static string configFilePath
    {
      get { return $"{AppContext.BaseDirectory}\\Data\\config.json"; }
    }

    public static Config InitializeConfigFromDisk()
    {
      string content = Utils.ReadFileContents(configFilePath);

      return JsonConvert.DeserializeObject<Config>(content);
    }

    public static bool SaveConfigToDisk(Config configToSave)
    {
      try
      {
        string fileContents = JsonConvert.SerializeObject(configToSave);
        File.WriteAllText(configFilePath, fileContents);
        return true;
      }
      catch 
      {
        return false;
      }
    }
  }
}