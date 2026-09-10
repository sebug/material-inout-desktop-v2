using material_inout_desktop_v2.ViewModels;
#if IOS || MACCATALYST
using WebKit;
#endif

namespace material_inout_desktop_v2.Pages;

public partial class VoucherDetailPage : ContentPage
{
	public VoucherDetailPage(VoucherDetailViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    private void voucherDetailWebView_HandlerChanged(object? sender, EventArgs e)
	{
		#if IOS || MACCATALYST
		if (voucherDetailWebView.Handler.PlatformView is WKWebView wkWebView)
		{
			
		}
		#endif
	}
}