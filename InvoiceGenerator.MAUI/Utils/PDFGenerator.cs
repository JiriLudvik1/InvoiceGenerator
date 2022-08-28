using InvoiceGenerator.MAUI.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfDocument = PdfSharp.Pdf.PdfDocument;

namespace InvoiceGenerator.MAUI
{
  public class PDFGenerator
  {
    #region Fonts
    private readonly XFont HeaderFont = new XFont("Arial", 18, XFontStyle.Bold);
    private readonly XFont NormalFont = new XFont("Arial", 12);
    private readonly XFont NormalBoldFont = new XFont("Arial", 12, XFontStyle.Bold);
    private readonly XFont FooterFont = new XFont("Arial", 10);
    #endregion

    private readonly Customer customer;
    private readonly InvoiceDetail invoiceDetail;
    private readonly Config config;
    private readonly string filePath;

    public PDFGenerator(Customer customer, InvoiceDetail invoiceDetail, Config config, string filePath)
    {
      this.customer = customer;
      this.invoiceDetail = invoiceDetail;
      this.config = config;
      this.filePath = filePath;
    }

    public bool GenerateAndSaveInvoicePDF()
    {
      try
      {
        // Create a new PDF document
        PdfDocument document = new PdfDocument();
        document.Info.Title = invoiceDetail.InvoiceName;
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Create a font
        XFont font = new XFont("Arial", 15);

        // Draw the text
        gfx.DrawString($"Počet zákazníků v db je: ", font, XBrushes.Black,
          new XRect(0, 0, page.Width, page.Height),
          XStringFormats.Center);



        document.Save(filePath);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
