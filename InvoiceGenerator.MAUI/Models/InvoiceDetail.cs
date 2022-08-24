namespace InvoiceGenerator.MAUI.Models
{
  public class InvoiceDetail
  {
    public string InvoiceNumber { get; set; }
    public DateOnly PaymentDue { get; set; }
    public DateOnly CreatedDate { get; set; }
    public DateOnly DateOfTaxableSupply { get; set; }
    public bool InstalationPreset { get; set; }
    public int PresetInstalationPrice { get; set; }
    public string Attachments { get; set; }
  }
}
