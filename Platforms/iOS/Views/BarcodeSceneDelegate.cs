
using ARKit;
using SceneKit;
using UIKit;

namespace material_inout_desktop_v2.Views;

public class BarcodeSceneDelegate : ARSCNViewDelegate
{
    public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
    {
        if (anchor.Name?.StartsWith("barcode:") != true)
        {
            return;
        }

        const float size = 0.05f; // 5 cm
        var box = SCNBox.Create(size, size, size, 0);
        var material = box.FirstMaterial!;
        material.Diffuse.Contents = UIColor.SystemBlue;
        material.Transparency = 0.5f;
        


        var boxNode = SCNNode.FromGeometry(box);
        // The raycast's Y axis is the surface normal, so lift the box to sit on the surface
        boxNode.Position = new SCNVector3(0, size / 2, 0);

        node.AddChildNode(boxNode);
    }
}