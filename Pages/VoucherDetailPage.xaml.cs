using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class VoucherDetailPage : ContentPage
{
	public VoucherDetailPage(VoucherDetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}