namespace material_inout_desktop_v2.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Message
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Message));
            }
        }
    } = "Material In/Out";
}