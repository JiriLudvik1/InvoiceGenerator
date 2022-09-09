using InvoiceGenerator.MAUI.Models;
using InvoiceGenerator.MAUI.ViewModels;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace InvoiceGenerator.MAUI
{
  public partial class MainPage : ContentPage
  {
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