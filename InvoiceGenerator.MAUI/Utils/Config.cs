using Newtonsoft.Json;

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
    public string DefaultReportPath { get; set; }

    public static Config InitializeConfigFromDisk()
    {
      string configFullPath = $"{AppContext.BaseDirectory}/Data/config.json";
      string content = Utils.ReadFileContents(configFullPath);

      return JsonConvert.DeserializeObject<Config>(content);

    }
  }

}