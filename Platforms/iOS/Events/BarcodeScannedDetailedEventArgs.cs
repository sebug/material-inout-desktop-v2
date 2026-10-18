using Vision;

namespace material_inout_desktop_v2.Events;

public class BarcodeScannedDetailedEventArgs : BarcodeScannedEventArgs
{
    public required VNBarcodeSymbology Symbology { get; init; }
}