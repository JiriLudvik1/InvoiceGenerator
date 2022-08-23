using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace InvoiceGenerator
{
  public class Config
  {
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string IC { get; set; }
    public string DIC { get; set; }
    public string AccountNumber { get; set; }
    public string DefaultReportPath { get; set; }

    public static Config InitializeFromDisk()
    {
      return GetConfigFile().Result;
    }

    private static async Task<Config> GetConfigFile()
    {
      StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
      StorageFolder data = await appInstalledFolder.GetFolderAsync("Data");

      string content = await Utils.ReadFileContents(await data.GetFileAsync("config.json"));

      return JsonConvert.DeserializeObject<Config>(content);

    }
  }
}