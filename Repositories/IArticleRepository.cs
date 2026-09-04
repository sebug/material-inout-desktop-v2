using material_inout_desktop_v2.Entities;

namespace material_inout_desktop_v2.Repositories;

public interface IArticleRepository
{
    Task EnsureArticle(Article article);
    Task<List<Article>> GetAllArticles();
    Task<Article?> GetByEAN(string ean);
    Task<Voucher> CreateVoucher(string name);

    Task<List<Voucher>> GetAllVouchers();

    Task<List<Voucher>> GetAllNonReturnedVouchers();

    Task<List<Voucher>> GetReturnedVouchers();

    Task<VoucherLine> AddVoucherLine(VoucherLine voucherLine);

    Task<VoucherLine> ReturnVoucherLine(int id, string returnText);

    Task<Voucher> GetVoucherById(int id);

    Task<Voucher> UpdateVoucher(Voucher voucher);

    Task<List<VoucherLine>> GetVoucherLinesByVoucherId(int voucherId);
}
