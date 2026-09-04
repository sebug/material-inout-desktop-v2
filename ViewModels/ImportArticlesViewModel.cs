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
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "org.openxmlformats.spreadsheetml.sheet" } }, // UTType values
                    { DevicePlatform.MacCatalyst, new[] { "org.openxmlformats.spreadsheetml.sheet" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".xlsx" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "xlsx" } }, // UTType values
                });
                var options = new PickOptions
                {
                    PickerTitle = "Importer fichier d'articles",
                    FileTypes = customFileType
                };
                var result = await FilePicker.Default.PickAsync(options);
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
                                await ArticleRepository.EnsureArticle(new Article
                                {
                                    Label = line.Label,
                                    Mnemonic = line.Mnemonic,
                                    EAN = line.EAN
                                });
                            }
                            var linesInRepository = await ArticleRepository.GetAllArticles();
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
        });
    }
}