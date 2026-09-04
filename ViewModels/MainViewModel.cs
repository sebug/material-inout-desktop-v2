using material_inout_desktop_v2.Entities;
using material_inout_desktop_v2.Repositories;

namespace material_inout_desktop_v2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IArticleRepository ArticleRepository;
    public MainViewModel(IArticleRepository articleRepository)
    {
        ArticleRepository = articleRepository;
    }

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

    public List<ArticleViewModel> Articles
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Articles));
            }
        }
    } = new List<ArticleViewModel>();

    public string BarCode
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                string ean = field;
                MainThread.BeginInvokeOnMainThread(async () => await ProcessEAN(ean));
                OnPropertyChanged(nameof(BarCode));
            }
        }
    } = String.Empty;

    private async Task ProcessEAN(string ean)
    {
        var articleList = Articles.ToList();
        if (!articleList.Any(art => art.Article.EAN == ean))
        {
            var article = await ArticleRepository.GetByEAN(ean);
            if (article == null)
            {
                await Shell.Current.DisplayAlertAsync("Errour de lecture",
                "Article pas trouvé dans la base de donnés", "OK");
                return;
            }
            var avm = new ArticleViewModel
            {
                Article = article,
                RemoveCommand = new Command((arg) =>
                {
                    var avm = arg as ArticleViewModel;
                    if (avm != null)
                    {
                        var listWithout = Articles.Where(art => art != avm).ToList();
                        Articles = listWithout;
                    }
                })
            };
            articleList.Add(avm);
        }
        Articles = articleList;
    }
}