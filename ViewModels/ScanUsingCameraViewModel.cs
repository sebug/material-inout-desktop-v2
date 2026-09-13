using System.Windows.Input;
using material_inout_desktop_v2.Views;

namespace material_inout_desktop_v2.ViewModels;

public class ScanUsingCameraViewModel : ViewModelBase
{
    public bool IsARActive
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(IsARActive));
            }
        }
    }

    public string StatusMessage
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
    } = String.Empty;

    public ICommand StartARCommand { get; }
    public ICommand StopARCommand { get; }

    public ScanUsingCameraViewModel()
    {
        StartARCommand = new Command(StartAR);
        StopARCommand = new Command (StopAR);
    }

    private void StartAR()
    {
        #if IOS
        if (MauiARView.IsARSupported())
        {
            IsARActive = true;
            StatusMessage = "Scan démarré";
        }
        else
        {
            StatusMessage = "AR unsupported on this device.";
        }
        #endif
    }

    private void StopAR()
    {
        #if IOS
        IsARActive = false;
        StatusMessage = "Scan arrêté.";
        #endif
    }
}
