using System.Collections.Concurrent;
using ARKit;
using CoreGraphics;
using material_inout_desktop_v2.Events;
using SceneKit;
using UIKit;

namespace material_inout_desktop_v2.Views;

public class MauiARView : UIView
{
    ARSCNView? _arView;
    ARSession? _arSession;

    BarcodeScannerARSessionDelegate? _barcodeScannerSessionDelegate;
    BarcodeSceneDelegate? _barcodeSceneDelegate;

    private HashSet<string> _placedPayloads = new HashSet<string>();

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
        _barcodeSceneDelegate = new BarcodeSceneDelegate();
        _arView.Delegate = _barcodeSceneDelegate;
        _arSession = view.Session;
        _barcodeScannerSessionDelegate = new BarcodeScannerARSessionDelegate();
        _arSession.Delegate = _barcodeScannerSessionDelegate;
        _barcodeScannerSessionDelegate.BarcodeScanned += OnBarcodeScanned;

        var tap = new UITapGestureRecognizer(HandleTap);
        _arView.AddGestureRecognizer(tap);
    }

    private void HandleTap(UITapGestureRecognizer gesture)
    {
        if (_arView == null)
        {
            return;
        }
        var location = gesture.LocationInView(_arView);
        var hits = _arView.HitTest(location, new SCNHitTestOptions());

        foreach (var hit in hits)
        {
            // Walk up the hierarchy in case you later add child nodes to the box
            for (var node = hit.Node; node != null; node = node.ParentNode)
            {
                if (node.Name?.StartsWith("barcode:") == true)
                {
                    var payload = node.Name.Substring("barcode:".Length);
                    OnBarcodeBoxTapped(node, payload);
                    return;
                }
            }
        }
    }

    private void OnBarcodeBoxTapped(SCNNode boxNode, string payload)
    {
        // Quick visual feedback: a little "bounce"
        boxNode.RunAction(SCNAction.Sequence(new[]
        {
            SCNAction.ScaleTo(1.2f, 0.1),
            SCNAction.ScaleTo(1.0f, 0.1)
        }));
    }

    /// <summary>
    /// On barcode scanned: Display an overlay for the barcode, and then bubble up the event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnBarcodeScanned(object? sender, BarcodeScannedDetailedEventArgs e)
    {
        lock (_placedPayloads)
        {
            // Only place one object per barcode
            if (!_placedPayloads.Add(e.BarcodeContent))
            {
                return;
            }
        }

        var camera = e.CameraSnapshot;
        var box = e.BoundingBox;
        var visionCenter = new CGPoint(box.GetMidX(), box.GetMidY());

        // Convert to the raw landscape buffer (origin top-left)
        var rawPoint = VisionPointToRawImagePoint(visionCenter);

        var (origin, direction) = RayForImagePoint(camera, rawPoint);

        UIApplication.SharedApplication.InvokeOnMainThread(() =>
        {
            var session = _arSession;
            if (session == null)
                return;

            using var query = new ARRaycastQuery(
                origin, direction, ARRaycastTarget.EstimatedPlane, ARRaycastTargetAlignment.Any);

            var hits = session.Raycast(query);
            if (hits.Length == 0)
            {
                // Nothing to hit yet (e.g. the surface isn't tracked); allow a retry on a later frame
                lock (_placedPayloads)
                {
                    _placedPayloads.Remove(e.BarcodeContent);
                }
                return;
            }

            var anchor = new ARAnchor($"barcode:{e.BarcodeContent}", hits[0].WorldTransform);
            session.AddAnchor(anchor);
        });

        BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
        {
            BarcodeContent = e.BarcodeContent
        });
    }

    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    void AddContent()
    {
    }

    /// Builds a world-space ray through a normalized point of the raw camera image.
    private static (NVector3 origin, NVector3 direction) RayForImagePoint(CameraSnapshot cam, CGPoint normalized)
    {
        // Normalized -> pixel coordinates in the captured image
        double px = normalized.X * cam.ImageSize.Width;
        double py = normalized.Y * cam.ImageSize.Height;

        // Intrinsics: fx, fy = focal lengths, cx, cy = principal point (in pixels)
        var k = cam.Intrinsics;
        float x = (float)((px - k.M13) / k.M11);
        float y = (float)(-(py - k.M23) / k.M22); // image y goes down, camera y goes up
        float z = -1f;                             // camera looks down -Z

        // Rotate the direction into world space using the camera transform
        var m = cam.Transform;
        var dx = m.M11 * x + m.M12 * y + m.M13 * z;
        var dy = m.M21 * x + m.M22 * y + m.M23 * z;
        var dz = m.M31 * x + m.M32 * y + m.M33 * z;
        var len = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

        var origin = new NVector3(m.M14, m.M24, m.M34);
        var direction = new NVector3(dx / len, dy / len, dz / len);
        return (origin, direction);
    }
    
    /// Vision point in the .Right-oriented image (origin bottom-left)
    /// -> normalized point in the raw landscape camera buffer (origin top-left).
    private static CGPoint VisionPointToRawImagePoint(CGPoint p)
    {
        return new CGPoint(1 - p.Y, 1 - p.X);
    }
}
