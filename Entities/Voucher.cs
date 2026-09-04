using SQLite;

namespace material_inout_desktop_v2.Entities;

public class Voucher
{
    [PrimaryKey, AutoIncrement]
    public int VoucherID { get; set; }

    public string Name { get; set; } = String.Empty;

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? ReturnedDate { get; set; }

    public string ReturningPersonName { get; set; } = String.Empty;

    [Ignore]
    public int VoucherLineCount { get; set; }
}
