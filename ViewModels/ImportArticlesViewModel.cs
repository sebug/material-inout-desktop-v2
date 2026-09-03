using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace material_inout_desktop_v2.ViewModels;

public class ImportArticlesViewModel : ViewModelBase
{
    public ImportArticlesViewModel()
    {
        ImportCommand = new Command(async () => await PerformImport());
    }

    public ICommand ImportCommand { get; }

    private async Task PerformImport()
    {
        await Shell.Current.DisplayAlertAsync("Import File", "Importing articles", "OK");
    }
}