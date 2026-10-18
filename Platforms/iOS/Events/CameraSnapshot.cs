using CoreGraphics;

namespace material_inout_desktop_v2.Events;

// Storing the information we need to map the scanned barcode bounding box to the real world
public readonly record struct CameraSnapshot(NMatrix4 Transform, NMatrix3 Intrinsics, CGSize ImageSize);