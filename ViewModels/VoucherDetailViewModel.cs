using System.Windows.Input;
using material_inout_desktop_v2.Repositories;
using material_inout_desktop_v2.Services;

namespace material_inout_desktop_v2.ViewModels;

public class VoucherDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly IReportService ReportService;
    private readonly IArticleRepository ArticleRepository;

    public VoucherDetailViewModel(IReportService reportService,
    IArticleRepository articleRepository)
    {
        ReportService = reportService;
        ArticleRepository = articleRepository;
        ReturnMaterialCommand = new Command(async () => await PerformReturnMaterial());
    }

    public int VoucherID
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                if (field > 0)
                {
                    MainThread.BeginInvokeOnMainThread(async () => await GenerateHTML());
                }
                OnPropertyChanged(nameof(VoucherID));
            }
        }
    }

    public string HTMLContent
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(HTMLContent));
            }
        }
    } = String.Empty;

    public bool ShowReturnMaterial
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ShowReturnMaterial));
            }
        }
    }

    public ICommand ReturnMaterialCommand { get; }

    private async Task PerformReturnMaterial()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await Shell.Current.GoToAsync("/ReturnMaterial", ((IDictionary<string, object>)new Dictionary<string, object>
                {
                    { "VoucherID", VoucherID.ToString() }
                }));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Erreur de retour de matériel: " + ex.ToString(), "OK");
            } 
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string? voucherIDString = query["VoucherID"] as string;
        if (!String.IsNullOrEmpty(voucherIDString) && int.TryParse(voucherIDString, out int vid))
        {
            VoucherID = vid;
        }
    }

    private async Task GenerateHTML()
    {
        try
        {
            HTMLContent = await ReportService.GenerateVoucherHTML(VoucherID);
            var voucher = await ArticleRepository.GetVoucherById(VoucherID);
            if (voucher != null)
            {
                ShowReturnMaterial = !voucher.ReturnedDate.HasValue;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
        }
    }
}
