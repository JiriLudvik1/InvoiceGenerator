using InvoiceGenerator.MAUI.ViewModels;

namespace InvoiceGenerator.MAUI;

public partial class SettingsPage : ContentPage
{

	public SettingsPageViewModel ViewModel { get; set; }

	public SettingsPage(Config configuration)
	{
		InitializeComponent();
		ViewModel = new SettingsPageViewModel(Navigation, configuration, ConfigurationGrid);
		BindingContext = ViewModel;
	}
}