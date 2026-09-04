using System.Windows.Input;
using material_inout_desktop_v2.Entities;

namespace material_inout_desktop_v2.ViewModels;

public class ArticleViewModel : ViewModelBase
{
    public required Article Article { get; init; }

    public required ICommand RemoveCommand { get; init; }
}
