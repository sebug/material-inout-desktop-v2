using System.Windows.Input;
using material_inout_desktop_v2.Entities;
using material_inout_desktop_v2.Repositories;

namespace material_inout_desktop_v2.ViewModels;

public class ReturnMaterialViewModel : ViewModelBase, IQueryAttributable
{
    private IArticleRepository ArticleRepository;
    public ReturnMaterialViewModel(IArticleRepository articleRepository)
    {
        ArticleRepository = articleRepository;
        ConfirmMaterialReturnCommand = new Command(async () => await ConfirmMaterialReturn());
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
                    MainThread.BeginInvokeOnMainThread(async () => await LoadVoucher());
                }
                OnPropertyChanged(nameof(VoucherID));
            }
        }
    }

    public string PersonResponsible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(PersonResponsible));
            }
        }
    } = String.Empty;

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

    public string ReturningPersonName
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(ReturningPersonName));
            }
        }
    } = String.Empty;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string? voucherIDString = query["VoucherID"] as string;
        if (!String.IsNullOrEmpty(voucherIDString) && int.TryParse(voucherIDString, out int vid))
        {
            VoucherID = vid;
        }
    }

    private async Task ProcessEAN(string ean)
    {
        var matchingLine = VoucherLines.FirstOrDefault(vl =>
        vl.VoucherLine.EAN == ean);
        if (matchingLine != null)
        {
            var voucherLinesAfter = VoucherLines
            .Select(vl =>
            {
                if (vl == matchingLine)
                {
                    return new VoucherLineViewModel
                    {
                        VoucherLine = new VoucherLine
                        {
                            VoucherLineID = vl.VoucherLine.VoucherLineID,
                            VoucherID = vl.VoucherLine.VoucherID,
                            EAN = vl.VoucherLine.EAN,
                            Label = vl.VoucherLine.Label,
                            ReturnStatus = "Retourné"
                        },
                        MarkAsReturnedCommand = vl.MarkAsReturnedCommand
                    };
                }
                else
                {
                    return vl;
                }
            }).ToList();
            VoucherLines = voucherLinesAfter;
        }
    }

    public List<VoucherLineViewModel> VoucherLines
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(nameof(VoucherLines));
            }
        }
    } = new List<VoucherLineViewModel>();

    private async Task LoadVoucher()
    {
        try
        {
            var voucher = await ArticleRepository.GetVoucherById(VoucherID);

            PersonResponsible = voucher.Name;

            var voucherLines = await ArticleRepository.GetVoucherLinesByVoucherId(VoucherID);

            VoucherLines = voucherLines.Select(entity => new VoucherLineViewModel
            {
                VoucherLine = entity,
                MarkAsReturnedCommand = new Command(async voucherLineVM =>
                {
                    if (voucherLineVM is VoucherLineViewModel vlvm)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (vlvm is VoucherLineViewModel voucherLineViewModel)
                            {
                                await ReturnManually(voucherLineViewModel);
                            }
                        });
                    }
                })
            }).ToList();

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Error fetching vouchers: " + ex.ToString(), "OK");
        }
    }

    private async Task ReturnManually(VoucherLineViewModel voucherLineViewModel)
    {
        try
        {
            var voucherLinesAfter = VoucherLines
            .Select(vl =>
            {
                if (vl == voucherLineViewModel)
                {
                    return new VoucherLineViewModel
                    {
                        VoucherLine = new VoucherLine
                        {
                            VoucherLineID = vl.VoucherLine.VoucherLineID,
                            VoucherID = vl.VoucherLine.VoucherID,
                            EAN = vl.VoucherLine.EAN,
                            Label = vl.VoucherLine.Label,
                            ReturnStatus = "Retour Manuel"
                        },
                        MarkAsReturnedCommand = vl.MarkAsReturnedCommand
                    };
                }
                else
                {
                    return vl;
                }
            }).ToList();
            VoucherLines = voucherLinesAfter;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Erreur de retour manuel: " + ex.ToString(), "OK");
        }
    }

    public ICommand ConfirmMaterialReturnCommand { get; }

    private async Task ConfirmMaterialReturn()
    {
        try
        {
            string returningPersonName = ReturningPersonName;
            if (String.IsNullOrEmpty(returningPersonName))
            {
                await Shell.Current.DisplayAlertAsync("Erreur", "Veuillez rentrer le nom de la personne qui confirme le retour", "OK");
                return;
            }
            if (VoucherLines.Any(vl => String.IsNullOrEmpty(vl.VoucherLine.ReturnStatus)))
            {
                var notReturnedLines = VoucherLines.Where(vl => String.IsNullOrEmpty(vl.VoucherLine.ReturnStatus))
                .ToList();
                await Shell.Current.DisplayAlertAsync("Erreur", "Pas retourné: " +
                String.Join(", ", notReturnedLines.Select(vl => vl.VoucherLine.Label)), "OK");
                return;
            }
            foreach (var voucherLine in VoucherLines)
            {
                await ArticleRepository.ReturnVoucherLine(voucherLine.VoucherLine.VoucherLineID, voucherLine.VoucherLine.ReturnStatus);
            }
            var voucher = await ArticleRepository.GetVoucherById(VoucherID);
            voucher.ReturnedDate = DateTimeOffset.Now;
            voucher.ReturningPersonName = returningPersonName;
            await ArticleRepository.UpdateVoucher(voucher);
            await Shell.Current.GoToAsync("/voucherdetail", ((IDictionary<string, object>)new Dictionary<string, object>
            {
                { "VoucherID", VoucherID.ToString() }
            }));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Erreur de retour de matériel: " + ex.ToString(), "OK");
        }
    }
}
