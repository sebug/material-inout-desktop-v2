using material_inout_desktop_v2.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace material_inout_desktop_v2;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Routing.RegisterRoute("ImportArticles", typeof(ImportArticlesPage));
		Routing.RegisterRoute("VoucherDetail", typeof(VoucherDetailPage));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}