using material_inout_desktop_v2.Entities;
using SQLite;

namespace material_inout_desktop_v2.Repositories;

public class MaterialInOutDatabase : IMaterialInOutDatabase
{
    protected SQLiteAsyncConnection? Database;
    /// <summary>
    /// We need to lock the initialization not that we get a database back with only half the tables intialized
    /// </summary>
    private readonly SemaphoreSlim _initializationLock = new SemaphoreSlim(1, 1);

    private async Task Init()
    {
        await _initializationLock.WaitAsync();
        try
        {
            if (Database != null)
            {
                return;
            }
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            await Database.CreateTableAsync<Article>();
            await Database.CreateTableAsync<Voucher>();
            await Database.CreateTableAsync<VoucherLine>();
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async Task EnsureArticle(Article article)
    {
        await Init();
        int result = 0;
        await Database!.RunInTransactionAsync(conn =>
        {
            var toInsert = conn.Table<Article>().FirstOrDefault(
                art => art.EAN == article.EAN
            );
            if (toInsert != null)
            {
                toInsert.Mnemonic = article.Mnemonic;
                toInsert.Label = article.Label;
                conn.Update(toInsert);
            }
            else
            {
                result = conn.Insert(article);
            }
        }); 
    }

    public async Task<List<Article>> GetAllArticles()
    {
        await Init();
        return await Database!.Table<Article>().ToListAsync();
    }

    public async Task<Article?> GetByEAN(string ean)
    {
        await Init();
        return await Database!.Table<Article>().FirstOrDefaultAsync(a => a.EAN == ean);
    }

    public async Task<Voucher> CreateVoucher(string name)
    {
        await Init();
        var voucher = new Voucher
        {
            Name = name,
            CreatedDate = DateTimeOffset.Now
        };
        await Database!.RunInTransactionAsync(conn =>
        {
            conn.Insert(voucher);
        });
        return voucher;
    }

    public async Task<List<Voucher>> GetAllVouchers()
    {
        await Init();
        var result = new List<Voucher>();
        await Database!.RunInTransactionAsync(conn =>
        {
            result = conn.Table<Voucher>().OrderByDescending(voucher => voucher.CreatedDate).ToList();
            foreach (var voucher in result)
            {
                voucher.VoucherLineCount =
                    conn.Table<VoucherLine>().Count(line => line.VoucherID == voucher.VoucherID);
            }
        });
        return result;
    }

    public async Task<List<Voucher>> GetAllNonReturnedVouchers()
    {
        await Init();
        var result = new List<Voucher>();
        await Database!.RunInTransactionAsync(conn =>
        {
           result = conn.Table<Voucher>()
            .ToList()
            .OrderByDescending(v => v.CreatedDate)
            .Where(v => !v.ReturnedDate.HasValue).ToList();
            foreach (var voucher in result)
            {
                voucher.VoucherLineCount =
                    conn.Table<VoucherLine>().Count(line => line.VoucherID == voucher.VoucherID);
            } 
        });
        return result;
    }

    public async Task<List<Voucher>> GetReturnedVouchers()
    {
        await Init();
        var result = new List<Voucher>();
        await Database!.RunInTransactionAsync(conn =>
        {
            result = conn.Table<Voucher>()
            .ToList()
            .OrderByDescending(v => v.ReturnedDate)
            .Where(v => v.ReturnedDate.HasValue).ToList();
            foreach (var voucher in result)
            {
                voucher.VoucherLineCount =
                    conn.Table<VoucherLine>().Count(line => line.VoucherID == voucher.VoucherID);
            } 
        });
        return result;
    }

    public async Task<VoucherLine> AddVoucherLine(VoucherLine voucherLine)
    {
        await Init();
        await Database!.InsertAsync(voucherLine);
        return voucherLine;
    }

    public async Task<VoucherLine> ReturnVoucherLine(int id, string returnText)
    {
        await Init();
        VoucherLine? voucherLine = null;
        await Database!.RunInTransactionAsync(conn =>
        {
            voucherLine = conn.Table<VoucherLine>().FirstOrDefault(vl => vl.VoucherID == id);
            if (voucherLine == null)
            {
                throw new Exception("Could not find voucher line " + id);
            }
            voucherLine.ReturnStatus = returnText;
            conn.Update(voucherLine); 
        });
        return voucherLine ?? throw new Exception("Could not find voucher line " + id);
    }

    public Task<Voucher> GetVoucherById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Voucher> UpdateVoucher(Voucher voucher)
    {
        throw new NotImplementedException();
    }

    public Task<List<VoucherLine>> GetVoucherLinesByVoucherId(int voucherId)
    {
        throw new NotImplementedException();
    }
}