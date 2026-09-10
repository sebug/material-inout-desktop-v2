#if IOS || MACCATALYST
using WebKit;
#endif

namespace material_inout_desktop_v2.Views;

public class MyWebView : WebView
{
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        #if IOS || MACCATALYST
        if (Handler != null &&  Handler.PlatformView is WKWebView wKWebView)
        {
            var scriptMessageHandler = new MyWebViewScriptMessageHandler(wKWebView);
            wKWebView.Configuration.UserContentController.AddScriptMessageHandler(scriptMessageHandler, "myWebViewHandler");
        }
        #endif
    }
}
