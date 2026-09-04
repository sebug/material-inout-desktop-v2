using material_inout_desktop_v2.Entities;
using material_inout_desktop_v2.Repositories;
using material_inout_desktop_v2.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace material_inout_desktop_v2.ViewModels;

public class ImportArticlesViewModel : ViewModelBase
{
    private readonly IArticlesListReader ArticlesListReader;
    private readonly IArticleRepository ArticleRepository;

    public ImportArticlesViewModel(IArticlesListReader articlesListReader,
        IArticleRepository articleRepository)
    {
        ArticlesListReader = articlesListReader;
        ArticleRepository = articleRepository;
        ImportCommand = new Command(async () => await PerformImport());
    }

    public ICommand ImportCommand { get; }

    private async Task PerformImport()
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Importer fichier d'articles"
            };
            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                if (!result.FileName.EndsWith(".xlsx"))
                {
                    throw new Exception("Veuillez choisir un fichier .xlsx");
                }
                using (var stream = await result.OpenReadAsync())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var bytes = ms.ToArray();
                    var lines = ArticlesListReader.ReadExcelFile(bytes);
                    try
                    {
                        var linesWithEAN = lines.Where(line => !String.IsNullOrEmpty(line.EAN))
                        .ToList();
                        foreach (var line in linesWithEAN)
                        {
                            ArticleRepository.EnsureArticle(new Article
                            {
                                Label = line.Label,
                                Mnemonic = line.Mnemonic,
                                EAN = line.EAN
                            });
                        }
                        var linesInRepository = ArticleRepository.GetAllArticles();
                        await Shell.Current.DisplayAlertAsync("Notification", "Number of lines in repository: " + linesInRepository.Count, "OK");
                    }
                    catch (Exception ex)
                    {
                        await Shell.Current.DisplayAlertAsync("Error", $"An error occurred while storing articles: {ex.Message} - {ex.StackTrace}", "OK");
                        if (ex.InnerException != null)
                        {
                            await Shell.Current.DisplayAlertAsync("Inner Error", ex.ToString(), "OK");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlertAsync("Erreur d'import de fichier", "Erreur d'import - " + ex.Message, "OK");
        }
    }
}