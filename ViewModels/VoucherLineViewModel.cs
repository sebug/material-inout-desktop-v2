using System.Windows.Input;
using material_inout_desktop_v2.Entities;

namespace material_inout_desktop_v2.ViewModels;

public class VoucherLineViewModel
{
    public required VoucherLine VoucherLine { get; init; }
    public required ICommand MarkAsReturnedCommand { get; init; }
}
