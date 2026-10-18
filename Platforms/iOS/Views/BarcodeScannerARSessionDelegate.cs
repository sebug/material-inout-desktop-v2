using ARKit;
using CoreGraphics;
using CoreVideo;
using Foundation;
using ImageIO;
using material_inout_desktop_v2.Events;
using UIKit;
using Vision;

namespace material_inout_desktop_v2.Views;

public class BarcodeScannerARSessionDelegate : ARSessionDelegate
{
    private int _isProcessing; // 0 = idle, 1 = busy
    private long _lastScanTicks;
    private static readonly long ScanIntervalTicks = TimeSpan.FromMilliseconds(150).Ticks;

    private readonly VNDetectBarcodesRequest _barcodesRequest;

    public BarcodeScannerARSessionDelegate()
    {
        _barcodesRequest = new VNDetectBarcodesRequest((_, _) => {})
        {
            Symbologies = new[] { VNBarcodeSymbology.Ean13 }
        };
    }

    public override void DidUpdateFrame(ARSession session, ARFrame frame)
    {
        // Always release the frame promptly, even if we skip it
        using (frame)
        {
            var now = DateTime.UtcNow.Ticks;
            if (now - _lastScanTicks < ScanIntervalTicks)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) != 0)
            {
                return; // still busy with the previous frame
            }

            _lastScanTicks = now;

            // Separate managed reference to the buffer, so the frame itself can be released now
            CVPixelBuffer pixelBuffer = frame.CapturedImage;
            CameraSnapshot snapshot;
            using (var camera = frame.Camera)
            {
                snapshot = new CameraSnapshot(camera.Transform, camera.Intrinsics, camera.ImageResolution);
            }

            Task.Run(() =>
            {
                try
                {
                    using var handler = new VNImageRequestHandler(
                        pixelBuffer,
                        CGImagePropertyOrientation.Right,
                        new VNImageOptions());

                    handler.Perform(new VNRequest[] { _barcodesRequest }, out NSError? error);
                    if (error != null)
                    {
                        Console.WriteLine($"Barcode request failed: {error.LocalizedDescription}");
                    }
                    else if (_barcodesRequest.Results is { Length: > 0 } results)
                    {
                        foreach (var barcode in results)
                        {
                            HandleBarcode(barcode, snapshot);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error performing detect barcode request - " + ex);
                }
                finally
                {
                    pixelBuffer.Dispose();
                    Interlocked.Exchange(ref _isProcessing, 0);
                }
            });
        }
    }

    private void HandleBarcode(VNBarcodeObservation barcode, CameraSnapshot camera)
    {
        var payload = barcode.PayloadStringValue;
        var symbology = barcode.Symbology;

        if (payload != null)
        {
            BarcodeScanned?.Invoke(this, new BarcodeScannedDetailedEventArgs
            {
                BarcodeContent = payload,
                Symbology = symbology,
                CameraSnapshot = camera,
                BoundingBox = barcode.BoundingBox
            });
        }
    }

    public event EventHandler<BarcodeScannedDetailedEventArgs>? BarcodeScanned;
}
