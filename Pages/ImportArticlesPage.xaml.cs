using material_inout_desktop_v2.ViewModels;

namespace material_inout_desktop_v2.Pages;

public partial class ImportArticlesPage : ContentPage
{
    public ImportArticlesPage(ImportArticlesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}