using System.ComponentModel;
using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class ReturnMaterialPage : ContentPage
{
	public ReturnMaterialPage(ReturnMaterialViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

		vm.PropertyChanged += vm_PropertyChanged;
	}

    private void vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReturnMaterialViewModel.VoucherLines))
		{
			barCodeInput.Text = String.Empty;
			barCodeInput.Focus();
		}
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
			if (BindingContext is ReturnMaterialViewModel vm)
			{
				vm.BarCode = text;
			}
			((Entry)sender).Text = String.Empty;
			((Entry)sender).Focus();
		}
	}
}