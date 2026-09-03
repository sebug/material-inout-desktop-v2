namespace material_inout_desktop_v2.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Title
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    } = "Matériel In/Out";
}