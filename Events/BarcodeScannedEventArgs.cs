namespace material_inout_desktop_v2.Events;

public class BarcodeScannedEventArgs : EventArgs
{
    public required string BarcodeContent { get; init; }
}