using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class ReturnMaterialPage : ContentPage
{
	public ReturnMaterialPage(ReturnMaterialViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}