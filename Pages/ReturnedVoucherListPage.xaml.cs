using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class ReturnedVoucherListPage : ContentPage
{
	public ReturnedVoucherListPage(ReturnedVoucherListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		if (BindingContext is ReturnedVoucherListViewModel vm)
		{
			Task.Run(async () => await vm.Init());
		}
    }
}