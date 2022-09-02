namespace InvoiceGenerator.MAUI;

public partial class SettingsPage : ContentPage
{
	public Config Configuration;
	public bool IsCanceled = true;

	public SettingsPage(Config configuration)
	{
		InitializeComponent();
		this.Configuration = configuration;
		FillWithData(this.Configuration);
	}

	private void FillWithData(Config configuration)
	{
		enName.Text = configuration.Name;
		enPhone.Text = configuration.PhoneNumber;
		enZLNumber.Text = configuration.ZLNumber;
		enIC.Text = configuration.IC;
		enDIC.Text = configuration.DIC;
		enAccountNumber.Text = configuration.AccountNumber;
		enBankName.Text = configuration.BankName;
		enStreet.Text = configuration.Street;
		enCity.Text = configuration.City;
		enZIPCode.Text = configuration.ZIPCode;
		enConstantSymbol.Text = configuration.ConstantSymbol;
		enDbLocation.Text = configuration.ConnectionString;
	}

	private void SaveConfiguration()
	{
		Configuration.Name = enName.Text;
		Configuration.PhoneNumber = enPhone.Text;
		Configuration.ZLNumber = enZLNumber.Text;
		Configuration.IC = enIC.Text;
		Configuration.DIC = enDIC.Text;
		Configuration.AccountNumber = enAccountNumber.Text;
		Configuration.BankName = enBankName.Text;
		Configuration.Street = enStreet.Text;
		Configuration.City = enCity.Text;
		Configuration.ZIPCode = enZIPCode.Text;
		Configuration.ConstantSymbol = enConstantSymbol.Text;
		Configuration.ConnectionString = enDbLocation.Text;
	}

	private bool ValidateInputs()
	{
		//TODO: finish this
		return true;
	}

	private async void btCancel_Clicked(object sender, EventArgs e)
	{
		IsCanceled = true;
		await Navigation.PopAsync();
	}

	private async void btConfirm_Clicked(object sender, EventArgs e)
	{
		IsCanceled = false;

		if (!ValidateInputs())
		{
			await DisplayAlert("Chyba", "Parametry nebyly správnì vyplnìny", "Zrušit");
			return;
		}
		SaveConfiguration();


		await Navigation.PopAsync();
  }

	private async void btFindDb_Clicked(object sender, EventArgs e)
	{
		var file = await Utils.PickFile();

		if (file is null)
		{
			return;
		}

		enDbLocation.Text = file.FullPath;
	}
}