namespace InvoiceGenerator.MAUI.Models
{
  public class InvoiceDetailModel
  {
    public int Number { get; set; }
    public string InvoiceName { get; set; }
    public DateTime PaymentDue { get; set; } = DateTime.Now;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime DateOfTaxableSupply { get; set; } = DateTime.Now;
    public bool InstalationPreset { get; set; }
    public double PresetInstalationPrice { get; set; }
    public string Attachments { get; set; }
  }
}
