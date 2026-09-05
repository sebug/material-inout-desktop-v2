using material_inout_desktop_v2.Entities;
using material_inout_desktop_v2.Repositories;

namespace material_inout_desktop_v2.ViewModels;

public class ReturnedVoucherListViewModel : ViewModelBase
{
    private readonly IArticleRepository ArticleRepository;
    public ReturnedVoucherListViewModel(IArticleRepository articleRepository)
    {
        ArticleRepository = articleRepository;
    }

    public List<VoucherViewModel> Vouchers
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(Vouchers));
            }
        }
    } = new List<VoucherViewModel>();

    public async Task Init()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                var vouchers = await ArticleRepository.GetReturnedVouchers();
                Vouchers = vouchers.Select(voucher => new VoucherViewModel
                {
                    Voucher = voucher,
                    OpenDetailsPageCommand = new Command(async () =>
                    {
                        await OpenDetailsPage(voucher);
                    })
                }).ToList();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
            }
        });
    }

    private async Task OpenDetailsPage(Voucher voucher)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            try
            {
                await Shell.Current.GoToAsync("/VoucherDetail", ((IDictionary<string, object>)new Dictionary<string, object>
                {
                    { "VoucherID", voucher.VoucherID.ToString() }
                }));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Error going to voucher detail: " + ex.ToString(), "OK");
            }
        });
    }
}
