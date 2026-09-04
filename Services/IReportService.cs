namespace material_inout_desktop_v2.Services;

public interface IReportService
{
    Task<string> GenerateVoucherHTML(int voucherID);
}
