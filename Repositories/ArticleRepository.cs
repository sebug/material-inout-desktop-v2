using material_inout_desktop_v2.Entities;

namespace material_inout_desktop_v2.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly IMaterialInOutDatabase Database;
    public ArticleRepository(MaterialInOutDatabase database)
    {
        Database = database;
    }

    public Task<VoucherLine> AddVoucherLine(VoucherLine voucherLine)
    {
        return Database.AddVoucherLine(voucherLine);
    }

    public Task<Voucher> CreateVoucher(string name)
    {
        return Database.CreateVoucher(name);
    }

    public Task EnsureArticle(Article article)
    {
        return Database.EnsureArticle(article);
    }

    public Task<List<Article>> GetAllArticles()
    {
        return Database.GetAllArticles();
    }

    public Task<List<Voucher>> GetAllNonReturnedVouchers()
    {
        return Database.GetAllNonReturnedVouchers();
    }

    public Task<List<Voucher>> GetAllVouchers()
    {
        return Database.GetAllVouchers();
    }

    public Task<Article?> GetByEAN(string ean)
    {
        return Database.GetByEAN(ean);
    }

    public Task<List<Voucher>> GetReturnedVouchers()
    {
        return Database.GetReturnedVouchers();
    }

    public Task<Voucher> GetVoucherById(int id)
    {
        return Database.GetVoucherById(id);
    }

    public Task<List<VoucherLine>> GetVoucherLinesByVoucherId(int voucherId)
    {
        return Database.GetVoucherLinesByVoucherId(voucherId);
    }

    public Task<VoucherLine> ReturnVoucherLine(int id, string returnText)
    {
        return Database.ReturnVoucherLine(id, returnText);
    }

    public Task<Voucher> UpdateVoucher(Voucher voucher)
    {
        return Database.UpdateVoucher(voucher);
    }
}
