using System.Windows.Input;
using material_inout_desktop_v2.Entities;

namespace material_inout_desktop_v2.ViewModels;

public class VoucherViewModel : ViewModelBase
{
    public required Voucher Voucher { get; init; }
    public required ICommand OpenDetailsPageCommand { get; init; }
}
