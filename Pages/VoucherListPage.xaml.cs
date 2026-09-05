using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class VoucherListPage : ContentPage
{
	public VoucherListPage(VoucherListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}