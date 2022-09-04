using InvoiceGenerator.MAUI.Models;
using InvoiceGenerator.MAUI.ViewModels;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace InvoiceGenerator.MAUI
{
  public partial class MainPage : ContentPage
  {
    public string ItemsCSVFilePath { get; set; }
    public DataTable ImportedItems { get; set; }
    public Config Configuration { get; set; }
    public DBQueries DBQueries { get; set; }
    public CustomerModel Customer { get; set; }
    public EmailService EmailService { get; set; }
    public MainPageViewModel ViewModel { get; set; }

    public MainPage()
    {
      InitializeComponent();
      ViewModel = new MainPageViewModel(Navigation);
      BindingContext = ViewModel;
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
  }
}