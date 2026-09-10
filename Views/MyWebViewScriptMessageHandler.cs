#if IOS || MACCATALYST
using WebKit;
using Foundation;
using UIKit;

namespace material_inout_desktop_v2.Views;


public class MyWebViewScriptMessageHandler : WKScriptMessageHandler
{
    private WKWebView _webView;
    public MyWebViewScriptMessageHandler(WKWebView webView)
    {
        _webView = webView;
    }

    override public void DidReceiveScriptMessage(WebKit.WKUserContentController userContentController, WebKit.WKScriptMessage message)
    {
        try
        {
            bool available = UIPrintInteractionController.PrintingAvailable;
            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.JobName = "Bon à imprimer";
            printInfo.OutputType = UIPrintInfoOutputType.General;

            var printController = UIPrintInteractionController.SharedPrintController;
            printController.PrintInfo = printInfo;
            printController.PrintFormatter = _webView.ViewPrintFormatter;
            printController.Present(true, (handler, completed, error) => {
                    if (error != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Printing failed: {error.LocalizedDescription}");
                    }
                });
        }
        catch (Exception ex)
        {
            
        }
    }
}
#endif