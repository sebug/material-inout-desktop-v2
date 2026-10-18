using CoreGraphics;
using Vision;

namespace material_inout_desktop_v2.Events;

public class BarcodeScannedDetailedEventArgs : BarcodeScannedEventArgs
{
    public required VNBarcodeSymbology Symbology { get; init; }
    public required CameraSnapshot CameraSnapshot { get; init; }
    public required CGRect BoundingBox { get; init; }
}