using ARKit;
using Microsoft.Maui.Handlers;

namespace material_inout_desktop_v2.Views;

public class ScanARViewHandler : ViewHandler<ScanARView, ARSCNView>
{
    private MauiARView? _mauiARView;

    public static IPropertyMapper<ScanARView, ScanARViewHandler> Mapper = new PropertyMapper<ScanARView, ScanARViewHandler>(ViewMapper)
    {
        [nameof(ScanARView.IsSessionRunning)] = MapIsSessionRunning
    };

    public ScanARViewHandler() : base(Mapper)
    {
        
    }

    protected override ARSCNView CreatePlatformView()
    {
        var arView = new ARSCNView()
        {
            AutoenablesDefaultLighting = true,
            DebugOptions = ARSCNDebugOptions.ShowWorldOrigin,
            ShowsStatistics = true
        };

        _mauiARView = new MauiARView();
        _mauiARView.SetARView(arView);

        return arView;
    }

    public static void MapIsSessionRunning(IViewHandler handler, IView view)
    {
        if (handler is ScanARViewHandler arHandler && view is ScanARView arView)
        {
            if (arView.IsSessionRunning)
            {
                arHandler._mauiARView?.StartARSession();   
            }
            else
            {
                arHandler._mauiARView?.StopARSession();
            }
        }
    }

    protected override void ConnectHandler(ARSCNView platformView)
    {
        base.ConnectHandler(platformView);

        if (VirtualView != null)
        {
            MapIsSessionRunning(this, VirtualView);
        }
    }

    protected override void DisconnectHandler(ARSCNView platformView)
    {
        _mauiARView?.StopARSession();

        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }
}
