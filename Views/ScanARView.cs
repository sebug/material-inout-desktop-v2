namespace material_inout_desktop_v2.Views;

public class ScanARView : View
{
    public static readonly BindableProperty IsSessionRunningProperty =
        BindableProperty.Create(nameof(IsSessionRunning), typeof(bool), typeof(ScanARView), false);

    public bool IsSessionRunning
    {
        get => (bool)GetValue(IsSessionRunningProperty);
        set => SetValue(IsSessionRunningProperty, value);
    }
}
