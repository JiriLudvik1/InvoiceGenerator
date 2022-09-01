using InvoiceGenerator.MAUI.Models;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace InvoiceGenerator.MAUI
{
  public class EmailService
  {
    private SmtpClient smtpClient;
    private MailAddress mailAddress;

    public EmailService(MailAddress mailAddress, string password, string host, int port)
    {
      this.mailAddress = mailAddress;
      smtpClient = new SmtpClient(host, port);
      smtpClient.EnableSsl = true;
      smtpClient.Credentials = new NetworkCredential(mailAddress.Address, password);
    }

    public bool SendInvoice(Customer customer, InvoiceDetail detail, string attachmentFilePath)
    {
      try
      {
        using (MailMessage message = new MailMessage())
        {
          message.From = mailAddress;
          message.To.Add(new MailAddress(customer.Email));

          message.Subject = $"Faktura: {detail.InvoiceName}";
          message.Body = GenerateMailBody();

          Attachment invoiceAttachment = new Attachment(attachmentFilePath, MediaTypeNames.Application.Octet);
          message.Attachments.Add(invoiceAttachment);

          smtpClient.Send(message);
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

    private string GenerateMailBody()
    {
      return String.Empty;
    }
  }
}
