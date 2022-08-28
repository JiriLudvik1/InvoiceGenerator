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
    private double leftMargin = 15;

    public PDFGenerator(Customer customer, InvoiceDetail invoiceDetail, Config config, string filePath)
    {
      this.customer = customer;
      this.invoiceDetail = invoiceDetail;
      this.config = config;
      this.filePath = filePath;
    }

    private double GetRowY(int rowNumber)
    {
      double rowHeight = 12;

      return rowNumber * rowHeight + rowHeight + 6;
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

        gfx.DrawString("FAKTURA-DAŇOVÝ DOKLAD", HeaderFont, XBrushes.Black, new XRect(0, 5, page.Width, 18), XStringFormats.Center);

        gfx.DrawRectangle(XPens.Black, 12, GetRowY(1), 150, GetRowY(16) + 5);
        gfx.DrawString("Dodavatel:", NormalBoldFont, XBrushes.Black, leftMargin, GetRowY(2));
        gfx.DrawString(config.Name, NormalFont, XBrushes.Black, leftMargin, GetRowY(3));
        gfx.DrawString(config.Street, NormalFont, XBrushes.Black, leftMargin, GetRowY(4));
        gfx.DrawString($"{config.ZIPCode} {config.City}", NormalFont, XBrushes.Black, leftMargin, GetRowY(5));

        gfx.DrawLine(XPens.Black, 12, GetRowY(6), 162, GetRowY(6));

        gfx.DrawString($"IČO:", NormalBoldFont, XBrushes.Black, leftMargin, GetRowY(7));
        gfx.DrawString(config.IC, NormalFont, XBrushes.Black, leftMargin + 50, GetRowY(7));

        gfx.DrawString($"DIČ:", NormalBoldFont, XBrushes.Black, leftMargin, GetRowY(8));
        gfx.DrawString(config.DIC, NormalFont, XBrushes.Black, leftMargin + 50, GetRowY(8));

        gfx.DrawString("Peněžní ústav:", NormalBoldFont, XBrushes.Black, leftMargin, GetRowY(10));
        gfx.DrawString(config.BankName, NormalFont, XBrushes.Black, leftMargin, GetRowY(11));

        gfx.DrawString("Číslo účtu:", NormalBoldFont, XBrushes.Black, leftMargin, GetRowY(12));
        gfx.DrawString(config.AccountNumber, NormalFont, XBrushes.Black, leftMargin, GetRowY(13));

        gfx.DrawLine(XPens.Black, 12, GetRowY(14), 162, GetRowY(14));

        gfx.DrawString($"Vystavil: {config.Name}", NormalFont, XBrushes.Black, leftMargin, GetRowY(15));
        gfx.DrawString($"Tel: {config.PhoneNumber}", NormalFont, XBrushes.Black, leftMargin, GetRowY(16));
        gfx.DrawString("elmax.brno@gmail.com", NormalFont, XBrushes.Black, leftMargin, GetRowY(17));
        gfx.DrawString($"ŽL. ev.č. {config.ZLNumber}", NormalFont, XBrushes.Black, leftMargin, GetRowY(18));



        gfx.DrawRectangle(XPens.Black, 162, GetRowY(1), page.Width - leftMargin - 162, GetRowY(14) + 5);
        double secondColumnX = 167;
        gfx.DrawString("Faktura č.", NormalBoldFont, XBrushes.Black, secondColumnX, GetRowY(2));
        gfx.DrawString(invoiceDetail.Number, NormalFont, XBrushes.Black, secondColumnX + 100, GetRowY(2));
        gfx.DrawString("Konst. symbol", NormalBoldFont, XBrushes.Black, secondColumnX, GetRowY(3));
        gfx.DrawString(config.ConstantSymbol, NormalFont, XBrushes.Black, secondColumnX + 100, GetRowY(3));


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
