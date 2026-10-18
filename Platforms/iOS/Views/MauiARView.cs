using ARKit;
using material_inout_desktop_v2.Events;
using UIKit;

namespace material_inout_desktop_v2.Views;

public class MauiARView : UIView
{
    ARSCNView? _arView;
    ARSession? _arSession;

    BarcodeScannerARSessionDelegate? _barcodeScannerSessionDelegate;

    public static bool IsARSupported()
    {
        return ARConfiguration.IsSupported;
    }

    public void StartARSession()
    {
        if (_arSession == null)
        {
            return;
        }

        _arSession.Run(new ARWorldTrackingConfiguration
        {
            AutoFocusEnabled = true,
            LightEstimationEnabled = true,
            PlaneDetection = ARPlaneDetection.Horizontal,
            WorldAlignment = ARWorldAlignment.GravityAndHeading
        }, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);

        AddContent();
    }

    public void StopARSession()
    {
        _arSession?.Pause();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _arSession?.Dispose();
            _arView?.Dispose();
            _barcodeScannerSessionDelegate?.Dispose();

            _arSession = null;
            _arView = null;
            _barcodeScannerSessionDelegate = null;
        }
        base.Dispose(disposing);
    }

    internal void SetARView(ARSCNView view)
    {
        _arView = view;
        _arSession = view.Session;
        _barcodeScannerSessionDelegate = new BarcodeScannerARSessionDelegate();
        _arSession.Delegate = _barcodeScannerSessionDelegate;
        _barcodeScannerSessionDelegate.BarcodeScanned += OnBarcodeScanned;
    }

    /// <summary>
    /// On barcode scanned: Display an overlay for the barcode, and then bubble up the event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBarcodeScanned(object? sender, BarcodeScannedDetailedEventArgs e)
    {
        BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
        {
            BarcodeContent = e.BarcodeContent
        });
    }

    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    void AddContent()
    {
    }
}
