using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class ScanUsingCameraPage : ContentPage
{
	public ScanUsingCameraPage(ScanUsingCameraViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}