using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2;

public partial class MainPage : ContentPage
{

	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}
