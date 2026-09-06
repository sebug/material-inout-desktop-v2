using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}

	protected override void OnAppearing()
	{
		Task.Run(async () =>
		{
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				barCodeInput.Focus();
				await Task.Delay(TimeSpan.FromSeconds(1));
				barCodeInput.Focus();
			});
		});
	}

	void OnBarCodeTextChanged(object? sender, EventArgs e)
	{
		if (sender == null)
		{
			return;
		}
		string text = ((Entry)sender).Text;
		if (text != null && text.Length >= 13)
		{
			if (BindingContext is MainViewModel vm)
			{
				vm.BarCode = text;
			}
			((Entry)sender).Text = String.Empty;
			((Entry)sender).Focus();
		}
	}
}
