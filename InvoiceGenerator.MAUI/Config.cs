using Newtonsoft.Json;

namespace InvoiceGenerator.MAUI
{
  public class Config
  {
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string IC { get; set; }
    public string DIC { get; set; }
    public string AccountNumber { get; set; }
    public string DefaultReportPath { get; set; }

    public static Config InitializeConfigFromDisk()
    {
      string configFullPath = $"{AppContext.BaseDirectory}/Data/config.json";
      string content = Utils.ReadFileContents(configFullPath);

      return JsonConvert.DeserializeObject<Config>(content);

    }
  }
}